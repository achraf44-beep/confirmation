"""
Facebook/Meta Graph API Integration for Wassel
Captures orders from Facebook Page conversations and comments
"""

import os
import logging
import hmac
import hashlib
from typing import List, Dict, Any, Optional
from datetime import datetime
from dotenv import load_dotenv
import requests

from wassel_client import WasselApiClient, generate_order_number

load_dotenv()

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


class FacebookIntegration:
    """Integration with Facebook Graph API for order capture"""
    
    GRAPH_API_URL = "https://graph.facebook.com/v18.0"
    
    def __init__(self):
        self.access_token = os.getenv("FACEBOOK_ACCESS_TOKEN")
        self.page_id = os.getenv("FACEBOOK_PAGE_ID")
        self.app_secret = os.getenv("FACEBOOK_APP_SECRET")
        self.wassel_client = WasselApiClient()
    
    def _make_request(self, endpoint: str, params: Optional[Dict] = None) -> Dict[str, Any]:
        """Make request to Facebook Graph API"""
        url = f"{self.GRAPH_API_URL}/{endpoint}"
        params = params or {}
        params["access_token"] = self.access_token
        
        response = requests.get(url, params=params)
        response.raise_for_status()
        return response.json()
    
    def verify_webhook_signature(self, payload: bytes, signature: str) -> bool:
        """Verify Facebook webhook signature"""
        if not self.app_secret:
            return False
        
        expected = hmac.new(
            self.app_secret.encode('utf-8'),
            payload,
            hashlib.sha256
        ).hexdigest()
        
        return hmac.compare_digest(f"sha256={expected}", signature)
    
    def get_page_conversations(self, limit: int = 25) -> List[Dict[str, Any]]:
        """Get recent conversations from Facebook Page"""
        endpoint = f"{self.page_id}/conversations"
        params = {
            "fields": "participants,messages{message,from,created_time}",
            "limit": limit
        }
        
        data = self._make_request(endpoint, params)
        return data.get("data", [])
    
    def get_page_feed(self, limit: int = 25) -> List[Dict[str, Any]]:
        """Get page feed posts and comments"""
        endpoint = f"{self.page_id}/feed"
        params = {
            "fields": "message,created_time,comments{message,from,created_time}",
            "limit": limit
        }
        
        data = self._make_request(endpoint, params)
        return data.get("data", [])
    
    def parse_order_from_message(self, message: str, sender_info: Dict) -> Optional[Dict[str, Any]]:
        """
        Parse order information from a message
        Expected format examples:
        - "Order: Product Name, Qty: 2, Wilaya: Alger, Phone: 0556000010"
        - Or structured JSON format
        """
        # This is a simplified parser - in production, use NLP or structured formats
        order_keywords = ["order", "commande", "طلب"]
        message_lower = message.lower()
        
        if not any(kw in message_lower for kw in order_keywords):
            return None
        
        # Try to extract phone number (Algerian format: 05XX XXX XXX)
        import re
        phone_match = re.search(r'0[567]\d{8}', message.replace(" ", ""))
        phone = phone_match.group(0) if phone_match else ""
        
        # Try to extract wilaya
        wilayas = ["alger", "oran", "constantine", "blida", "setif", "batna"]
        wilaya = next((w.title() for w in wilayas if w in message_lower), "Alger")
        
        return {
            "orderNumber": generate_order_number(),
            "source": "Facebook",
            "customerName": sender_info.get("name", "Facebook Customer"),
            "phoneNumber": phone,
            "wilaya": wilaya,
            "commune": f"{wilaya} Centre",
            "deliveryAddress": "À confirmer",
            "items": [{
                "productName": "Produit Facebook",
                "quantity": 1,
                "unitPrice": 0
            }],
            "note": f"Message Facebook: {message[:200]}"
        }
    
    def process_conversations(self) -> Dict[str, int]:
        """Process conversations and create orders"""
        conversations = self.get_page_conversations()
        
        # Login to Wassel
        admin_email = os.getenv("WASSEL_ADMIN_EMAIL", "admin@wassel.dz")
        admin_password = os.getenv("WASSEL_ADMIN_PASSWORD", "Admin123!")
        self.wassel_client.login(admin_email, admin_password)
        
        stats = {"processed": 0, "orders_created": 0, "errors": 0}
        
        for conv in conversations:
            messages = conv.get("messages", {}).get("data", [])
            participants = conv.get("participants", {}).get("data", [])
            
            # Get customer (non-page participant)
            customer = next(
                (p for p in participants if p.get("id") != self.page_id),
                {"name": "Unknown", "id": ""}
            )
            
            for msg in messages:
                stats["processed"] += 1
                message_text = msg.get("message", "")
                
                order_data = self.parse_order_from_message(message_text, customer)
                if order_data:
                    try:
                        self.wassel_client.create_order(order_data)
                        stats["orders_created"] += 1
                        logger.info(f"Created order from Facebook: {order_data['orderNumber']}")
                    except Exception as e:
                        stats["errors"] += 1
                        logger.error(f"Error creating order: {e}")
        
        logger.info(f"Facebook sync complete: {stats}")
        return stats
    
    def send_message(self, recipient_id: str, message: str) -> bool:
        """Send message to a user via Facebook Page"""
        url = f"{self.GRAPH_API_URL}/{self.page_id}/messages"
        
        data = {
            "recipient": {"id": recipient_id},
            "message": {"text": message},
            "access_token": self.access_token
        }
        
        try:
            response = requests.post(url, json=data)
            response.raise_for_status()
            return True
        except Exception as e:
            logger.error(f"Error sending Facebook message: {e}")
            return False


# Webhook handler for real-time updates
def handle_webhook(payload: Dict[str, Any]) -> None:
    """Handle incoming Facebook webhook events"""
    integration = FacebookIntegration()
    
    for entry in payload.get("entry", []):
        messaging = entry.get("messaging", [])
        
        for event in messaging:
            sender_id = event.get("sender", {}).get("id")
            message = event.get("message", {}).get("text", "")
            
            if message:
                logger.info(f"Received message from {sender_id}: {message}")
                
                # Process as potential order
                order_data = integration.parse_order_from_message(
                    message,
                    {"id": sender_id, "name": f"Facebook User {sender_id}"}
                )
                
                if order_data:
                    try:
                        admin_email = os.getenv("WASSEL_ADMIN_EMAIL", "admin@wassel.dz")
                        admin_password = os.getenv("WASSEL_ADMIN_PASSWORD", "Admin123!")
                        integration.wassel_client.login(admin_email, admin_password)
                        integration.wassel_client.create_order(order_data)
                        
                        # Send confirmation
                        integration.send_message(
                            sender_id,
                            f"Merci! Votre commande {order_data['orderNumber']} a été reçue. "
                            f"Notre équipe vous contactera bientôt pour confirmation."
                        )
                    except Exception as e:
                        logger.error(f"Error processing order: {e}")


if __name__ == "__main__":
    integration = FacebookIntegration()
    
    if integration.access_token and integration.page_id:
        # Process existing conversations
        # stats = integration.process_conversations()
        # print(f"Processing results: {stats}")
        pass
    else:
        print("Facebook Integration: Configure FACEBOOK_ACCESS_TOKEN and FACEBOOK_PAGE_ID in .env")
