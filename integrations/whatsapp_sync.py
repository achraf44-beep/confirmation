"""
WhatsApp Business API Integration for Wassel
Captures orders from WhatsApp conversations using Meta Cloud API
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


class WhatsAppIntegration:
    """Integration with WhatsApp Business Cloud API"""
    
    API_URL = "https://graph.facebook.com/v18.0"
    
    def __init__(self):
        self.access_token = os.getenv("WHATSAPP_ACCESS_TOKEN")
        self.phone_number_id = os.getenv("WHATSAPP_PHONE_NUMBER_ID")
        self.business_account_id = os.getenv("WHATSAPP_BUSINESS_ACCOUNT_ID")
        self.webhook_verify_token = os.getenv("WHATSAPP_WEBHOOK_VERIFY_TOKEN")
        self.wassel_client = WasselApiClient()
    
    def _make_request(self, method: str, endpoint: str, data: Optional[Dict] = None) -> Dict[str, Any]:
        """Make request to WhatsApp Cloud API"""
        url = f"{self.API_URL}/{endpoint}"
        headers = {
            "Authorization": f"Bearer {self.access_token}",
            "Content-Type": "application/json"
        }
        
        if method == "GET":
            response = requests.get(url, headers=headers)
        else:
            response = requests.post(url, headers=headers, json=data)
        
        response.raise_for_status()
        return response.json()
    
    def verify_webhook(self, mode: str, token: str, challenge: str) -> Optional[str]:
        """Verify WhatsApp webhook subscription"""
        if mode == "subscribe" and token == self.webhook_verify_token:
            logger.info("WhatsApp webhook verified")
            return challenge
        return None
    
    def send_text_message(self, to: str, message: str) -> Dict[str, Any]:
        """Send a text message via WhatsApp"""
        endpoint = f"{self.phone_number_id}/messages"
        
        data = {
            "messaging_product": "whatsapp",
            "to": to,
            "type": "text",
            "text": {"body": message}
        }
        
        return self._make_request("POST", endpoint, data)
    
    def send_template_message(self, to: str, template_name: str, language_code: str = "fr") -> Dict[str, Any]:
        """Send a template message via WhatsApp"""
        endpoint = f"{self.phone_number_id}/messages"
        
        data = {
            "messaging_product": "whatsapp",
            "to": to,
            "type": "template",
            "template": {
                "name": template_name,
                "language": {"code": language_code}
            }
        }
        
        return self._make_request("POST", endpoint, data)
    
    def send_order_confirmation(self, to: str, order_number: str, items: List[str], total: float) -> Dict[str, Any]:
        """Send order confirmation message"""
        items_text = "\n".join([f"• {item}" for item in items])
        
        message = (
            f"✅ *Commande Confirmée*\n\n"
            f"📦 Numéro: {order_number}\n\n"
            f"*Produits:*\n{items_text}\n\n"
            f"💰 *Total: {total} DA*\n\n"
            f"Notre équipe vous contactera pour confirmer la livraison.\n"
            f"Merci pour votre confiance! 🙏"
        )
        
        return self.send_text_message(to, message)
    
    def send_status_update(self, to: str, order_number: str, status: str) -> Dict[str, Any]:
        """Send order status update"""
        status_messages = {
            "Pending": "🕐 En attente de confirmation",
            "Confirmed": "✅ Confirmée",
            "Shipped": "🚚 Expédiée",
            "Delivered": "📦 Livrée",
            "Returned": "↩️ Retournée",
            "Cancelled": "❌ Annulée"
        }
        
        status_text = status_messages.get(status, status)
        
        message = (
            f"📋 *Mise à jour de commande*\n\n"
            f"Commande: {order_number}\n"
            f"Statut: {status_text}"
        )
        
        return self.send_text_message(to, message)
    
    def parse_order_from_message(self, message: str, sender_phone: str, sender_name: str = "") -> Optional[Dict[str, Any]]:
        """
        Parse order information from WhatsApp message
        Expected formats:
        - "Commande: [Produit], [Wilaya], [Adresse]"
        - Or interactive button responses
        """
        import re
        
        order_keywords = ["order", "commande", "طلب", "commander"]
        message_lower = message.lower()
        
        if not any(kw in message_lower for kw in order_keywords):
            return None
        
        # Extract wilaya (looking for Algerian wilaya names)
        wilayas = [
            "adrar", "chlef", "laghouat", "oum el bouaghi", "batna", "bejaia", "biskra",
            "bechar", "blida", "bouira", "tamanrasset", "tebessa", "tlemcen", "tiaret",
            "tizi ouzou", "alger", "djelfa", "jijel", "setif", "saida", "skikda",
            "sidi bel abbes", "annaba", "guelma", "constantine", "medea", "mostaganem",
            "msila", "mascara", "ouargla", "oran"
        ]
        
        wilaya = "Alger"
        for w in wilayas:
            if w in message_lower:
                wilaya = w.title()
                break
        
        # Clean phone number (remove WhatsApp country code format)
        clean_phone = sender_phone.replace("+213", "0")
        if not clean_phone.startswith("0"):
            clean_phone = "0" + clean_phone[-9:]
        
        return {
            "orderNumber": generate_order_number(),
            "source": "WhatsApp",
            "customerName": sender_name or f"Client WhatsApp",
            "phoneNumber": clean_phone,
            "wilaya": wilaya,
            "commune": f"{wilaya} Centre",
            "deliveryAddress": "À confirmer par téléphone",
            "items": [{
                "productName": "Produit WhatsApp",
                "quantity": 1,
                "unitPrice": 0
            }],
            "note": f"Message WhatsApp: {message[:300]}"
        }


def handle_webhook(payload: Dict[str, Any]) -> List[Dict[str, Any]]:
    """
    Handle incoming WhatsApp webhook events
    Returns list of processed messages
    """
    integration = WhatsAppIntegration()
    processed = []
    
    for entry in payload.get("entry", []):
        changes = entry.get("changes", [])
        
        for change in changes:
            value = change.get("value", {})
            messages = value.get("messages", [])
            contacts = value.get("contacts", [])
            
            # Build contact lookup
            contact_map = {
                c.get("wa_id"): c.get("profile", {}).get("name", "")
                for c in contacts
            }
            
            for msg in messages:
                msg_type = msg.get("type")
                sender = msg.get("from")
                sender_name = contact_map.get(sender, "")
                
                if msg_type == "text":
                    text = msg.get("text", {}).get("body", "")
                    logger.info(f"WhatsApp message from {sender}: {text}")
                    
                    # Parse as potential order
                    order_data = integration.parse_order_from_message(text, sender, sender_name)
                    
                    if order_data:
                        try:
                            # Login to Wassel
                            admin_email = os.getenv("WASSEL_ADMIN_EMAIL", "admin@wassel.dz")
                            admin_password = os.getenv("WASSEL_ADMIN_PASSWORD", "Admin123!")
                            integration.wassel_client.login(admin_email, admin_password)
                            
                            # Create order
                            result = integration.wassel_client.create_order(order_data)
                            logger.info(f"Created order from WhatsApp: {order_data['orderNumber']}")
                            
                            # Send confirmation
                            integration.send_order_confirmation(
                                sender,
                                order_data['orderNumber'],
                                [i['productName'] for i in order_data['items']],
                                sum(i['unitPrice'] * i['quantity'] for i in order_data['items'])
                            )
                            
                            processed.append({
                                "type": "order_created",
                                "order_number": order_data['orderNumber'],
                                "from": sender
                            })
                            
                        except Exception as e:
                            logger.error(f"Error processing WhatsApp order: {e}")
                            
                            # Send error message
                            integration.send_text_message(
                                sender,
                                "Désolé, une erreur s'est produite. Veuillez réessayer ou contacter notre support."
                            )
                    else:
                        # Auto-reply for non-order messages
                        integration.send_text_message(
                            sender,
                            "Bonjour! 👋\n\n"
                            "Pour passer une commande, envoyez:\n"
                            "*Commande: [Produit], [Wilaya], [Adresse]*\n\n"
                            "Exemple:\n"
                            "_Commande: T-shirt Premium, Alger, 123 Rue Didouche Mourad_"
                        )
                
                elif msg_type == "interactive":
                    # Handle interactive message responses (buttons, lists)
                    interactive = msg.get("interactive", {})
                    response_type = interactive.get("type")
                    
                    if response_type == "button_reply":
                        button_id = interactive.get("button_reply", {}).get("id")
                        logger.info(f"Button response from {sender}: {button_id}")
                        processed.append({
                            "type": "button_response",
                            "button_id": button_id,
                            "from": sender
                        })
    
    return processed


if __name__ == "__main__":
    integration = WhatsAppIntegration()
    
    if integration.access_token and integration.phone_number_id:
        # Test sending a message
        # integration.send_text_message("213XXXXXXXXX", "Test message from Wassel")
        pass
    else:
        print("WhatsApp Integration: Configure WHATSAPP_ACCESS_TOKEN and WHATSAPP_PHONE_NUMBER_ID in .env")
