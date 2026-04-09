"""
Google Sheets Integration for Wassel
Syncs orders between Google Sheets and Wassel API
"""

import os
import logging
from typing import List, Dict, Any, Optional
from datetime import datetime
from google.oauth2.service_account import Credentials
from googleapiclient.discovery import build
from googleapiclient.errors import HttpError
from dotenv import load_dotenv

from wassel_client import WasselApiClient, generate_order_number

load_dotenv()

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Google Sheets API scopes
SCOPES = ['https://www.googleapis.com/auth/spreadsheets']


class GoogleSheetsIntegration:
    """Integration with Google Sheets for order import/export"""
    
    def __init__(self):
        self.credentials_path = os.getenv("GOOGLE_SHEETS_CREDENTIALS_PATH")
        self.spreadsheet_id = os.getenv("GOOGLE_SHEETS_SPREADSHEET_ID")
        self.range_name = os.getenv("GOOGLE_SHEETS_RANGE", "Orders!A:K")
        self.service = None
        self.wassel_client = WasselApiClient()
        
        # Expected column mapping
        self.column_mapping = {
            0: "order_number",
            1: "customer_name",
            2: "phone_number",
            3: "wilaya",
            4: "commune",
            5: "delivery_address",
            6: "product_name",
            7: "quantity",
            8: "unit_price",
            9: "note",
            10: "status"  # For export
        }
    
    def authenticate(self):
        """Authenticate with Google Sheets API"""
        if not self.credentials_path or not os.path.exists(self.credentials_path):
            raise ValueError(f"Credentials file not found: {self.credentials_path}")
        
        credentials = Credentials.from_service_account_file(
            self.credentials_path, scopes=SCOPES
        )
        self.service = build('sheets', 'v4', credentials=credentials)
        logger.info("Google Sheets API authenticated successfully")
    
    def read_orders(self) -> List[Dict[str, Any]]:
        """Read orders from Google Sheets"""
        if not self.service:
            self.authenticate()
        
        try:
            sheet = self.service.spreadsheets()
            result = sheet.values().get(
                spreadsheetId=self.spreadsheet_id,
                range=self.range_name
            ).execute()
            
            values = result.get('values', [])
            
            if not values:
                logger.info("No data found in spreadsheet")
                return []
            
            # Skip header row
            orders = []
            for row in values[1:]:
                if len(row) < 9:  # Minimum required columns
                    continue
                
                order = {
                    "orderNumber": row[0] if row[0] else generate_order_number(),
                    "source": "GoogleSheets",
                    "customerName": row[1],
                    "phoneNumber": row[2],
                    "wilaya": row[3],
                    "commune": row[4],
                    "deliveryAddress": row[5],
                    "items": [{
                        "productName": row[6],
                        "quantity": int(row[7]) if row[7] else 1,
                        "unitPrice": float(row[8]) if row[8] else 0
                    }],
                    "note": row[9] if len(row) > 9 else None
                }
                orders.append(order)
            
            logger.info(f"Read {len(orders)} orders from Google Sheets")
            return orders
            
        except HttpError as error:
            logger.error(f"Error reading from Google Sheets: {error}")
            raise
    
    def sync_orders_to_wassel(self) -> Dict[str, int]:
        """Sync orders from Google Sheets to Wassel API"""
        orders = self.read_orders()
        
        # Login to Wassel API
        admin_email = os.getenv("WASSEL_ADMIN_EMAIL", "admin@wassel.dz")
        admin_password = os.getenv("WASSEL_ADMIN_PASSWORD", "Admin123!")
        self.wassel_client.login(admin_email, admin_password)
        
        # Get existing orders to avoid duplicates
        existing_orders = self.wassel_client.get_orders()
        existing_numbers = {o["orderNumber"] for o in existing_orders}
        
        stats = {"created": 0, "skipped": 0, "errors": 0}
        
        for order in orders:
            if order["orderNumber"] in existing_numbers:
                stats["skipped"] += 1
                continue
            
            try:
                self.wassel_client.create_order(order)
                stats["created"] += 1
                logger.info(f"Created order: {order['orderNumber']}")
            except Exception as e:
                stats["errors"] += 1
                logger.error(f"Error creating order {order['orderNumber']}: {e}")
        
        logger.info(f"Sync complete: {stats}")
        return stats
    
    def export_orders_to_sheet(self, orders: Optional[List[Dict]] = None) -> int:
        """Export orders from Wassel to Google Sheets"""
        if not self.service:
            self.authenticate()
        
        if orders is None:
            admin_email = os.getenv("WASSEL_ADMIN_EMAIL", "admin@wassel.dz")
            admin_password = os.getenv("WASSEL_ADMIN_PASSWORD", "Admin123!")
            self.wassel_client.login(admin_email, admin_password)
            orders = self.wassel_client.get_orders()
        
        # Prepare data for sheets
        header = [
            "Order Number", "Customer Name", "Phone", "Wilaya", "Commune",
            "Address", "Product", "Quantity", "Price", "Note", "Status", "Date"
        ]
        
        rows = [header]
        for order in orders:
            row = [
                order.get("orderNumber", ""),
                order.get("customerName", ""),
                order.get("phoneNumber", ""),
                order.get("wilaya", ""),
                order.get("commune", ""),
                order.get("deliveryAddress", ""),
                ", ".join([i.get("productName", "") for i in order.get("items", [])]),
                sum(i.get("quantity", 0) for i in order.get("items", [])),
                order.get("subtotal", 0),
                order.get("note", ""),
                order.get("status", ""),
                order.get("createdAtUtc", "")
            ]
            rows.append(row)
        
        try:
            body = {'values': rows}
            result = self.service.spreadsheets().values().update(
                spreadsheetId=self.spreadsheet_id,
                range="Export!A1",
                valueInputOption="USER_ENTERED",
                body=body
            ).execute()
            
            updated = result.get('updatedRows', 0)
            logger.info(f"Exported {updated} rows to Google Sheets")
            return updated
            
        except HttpError as error:
            logger.error(f"Error exporting to Google Sheets: {error}")
            raise


if __name__ == "__main__":
    integration = GoogleSheetsIntegration()
    
    # Example: Sync orders from Google Sheets to Wassel
    # stats = integration.sync_orders_to_wassel()
    # print(f"Sync results: {stats}")
    
    print("Google Sheets Integration ready. Configure .env and run sync methods.")
