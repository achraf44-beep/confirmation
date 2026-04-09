"""
Wassel API Client
Python client for interacting with the Wassel .NET API
"""

import os
import requests
from typing import Optional, List, Dict, Any
from dataclasses import dataclass
from datetime import datetime
from dotenv import load_dotenv

load_dotenv()

@dataclass
class Order:
    id: str
    order_number: str
    source: str
    customer_name: str
    phone_number: str
    wilaya: str
    commune: str
    delivery_address: str
    status: str
    subtotal: float
    created_at: datetime
    note: Optional[str] = None
    items: Optional[List[Dict]] = None


class WasselApiClient:
    """Client for Wassel API interactions"""
    
    def __init__(self, base_url: Optional[str] = None, token: Optional[str] = None):
        self.base_url = base_url or os.getenv("WASSEL_API_URL", "http://localhost:5000")
        self.token = token or os.getenv("WASSEL_API_TOKEN")
        self.session = requests.Session()
        if self.token:
            self.session.headers.update({"Authorization": f"Bearer {self.token}"})
    
    def _make_request(self, method: str, endpoint: str, **kwargs) -> requests.Response:
        """Make HTTP request to API"""
        url = f"{self.base_url}/api/{endpoint}"
        response = self.session.request(method, url, **kwargs)
        response.raise_for_status()
        return response
    
    def login(self, email: str, password: str) -> Dict[str, Any]:
        """Login and get JWT token"""
        response = self._make_request("POST", "auth/login", json={
            "email": email,
            "password": password
        })
        data = response.json()
        self.token = data["accessToken"]
        self.session.headers.update({"Authorization": f"Bearer {self.token}"})
        return data
    
    def get_orders(self) -> List[Dict[str, Any]]:
        """Get all orders"""
        response = self._make_request("GET", "orders")
        return response.json()
    
    def get_order(self, order_id: str) -> Dict[str, Any]:
        """Get order by ID"""
        response = self._make_request("GET", f"orders/{order_id}")
        return response.json()
    
    def create_order(self, order_data: Dict[str, Any]) -> Dict[str, Any]:
        """Create a new order"""
        response = self._make_request("POST", "orders", json=order_data)
        return response.json()
    
    def update_order_status(self, order_id: str, status: str, note: Optional[str] = None) -> bool:
        """Update order status"""
        data = {"status": status}
        if note:
            data["note"] = note
        response = self._make_request("PUT", f"orders/{order_id}/status", json=data)
        return response.status_code == 204
    
    def get_customers(self) -> List[Dict[str, Any]]:
        """Get all customers"""
        response = self._make_request("GET", "customers")
        return response.json()
    
    def create_customer(self, customer_data: Dict[str, Any]) -> Dict[str, Any]:
        """Create a new customer"""
        response = self._make_request("POST", "customers", json=customer_data)
        return response.json()
    
    def get_products(self) -> List[Dict[str, Any]]:
        """Get all products"""
        response = self._make_request("GET", "products")
        return response.json()
    
    def get_delivery_companies(self) -> List[Dict[str, Any]]:
        """Get all delivery companies"""
        response = self._make_request("GET", "deliverycompanies")
        return response.json()
    
    def get_wilayas(self) -> List[Dict[str, Any]]:
        """Get all Algerian wilayas"""
        response = self._make_request("GET", "locations/wilayas")
        return response.json()
    
    def get_dashboard_stats(self) -> Dict[str, Any]:
        """Get dashboard statistics"""
        response = self._make_request("GET", "dashboard/stats")
        return response.json()


def generate_order_number() -> str:
    """Generate unique order number"""
    import random
    return f"#{random.randint(10000, 99999)}"


if __name__ == "__main__":
    # Test client
    client = WasselApiClient()
    
    # Login
    try:
        auth = client.login("admin@wassel.dz", "Admin123!")
        print(f"Logged in as: {auth['user']['email']}")
        
        # Get stats
        stats = client.get_dashboard_stats()
        print(f"Total orders: {stats['totalOrders']}")
        print(f"Confirmation rate: {stats['confirmationRate']}%")
        
    except Exception as e:
        print(f"Error: {e}")
