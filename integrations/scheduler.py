"""
Sync Scheduler for Wassel Integrations
Runs periodic syncs for all integration sources
"""

import os
import logging
import time
from datetime import datetime
from apscheduler.schedulers.blocking import BlockingScheduler
from apscheduler.triggers.interval import IntervalTrigger
from dotenv import load_dotenv

from google_sheets_sync import GoogleSheetsIntegration
from facebook_sync import FacebookIntegration
from wassel_client import WasselApiClient

load_dotenv()

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)


def sync_google_sheets():
    """Sync orders from Google Sheets"""
    logger.info("Starting Google Sheets sync...")
    try:
        integration = GoogleSheetsIntegration()
        stats = integration.sync_orders_to_wassel()
        logger.info(f"Google Sheets sync complete: {stats}")
    except Exception as e:
        logger.error(f"Google Sheets sync failed: {e}")


def sync_facebook():
    """Sync orders from Facebook Page"""
    logger.info("Starting Facebook sync...")
    try:
        integration = FacebookIntegration()
        stats = integration.process_conversations()
        logger.info(f"Facebook sync complete: {stats}")
    except Exception as e:
        logger.error(f"Facebook sync failed: {e}")


def export_to_sheets():
    """Export orders to Google Sheets"""
    logger.info("Starting export to Google Sheets...")
    try:
        integration = GoogleSheetsIntegration()
        count = integration.export_orders_to_sheet()
        logger.info(f"Exported {count} orders to Google Sheets")
    except Exception as e:
        logger.error(f"Export to Google Sheets failed: {e}")


def health_check():
    """Check if Wassel API is healthy"""
    try:
        client = WasselApiClient()
        admin_email = os.getenv("WASSEL_ADMIN_EMAIL", "admin@wassel.dz")
        admin_password = os.getenv("WASSEL_ADMIN_PASSWORD", "Admin123!")
        client.login(admin_email, admin_password)
        stats = client.get_dashboard_stats()
        logger.info(f"Health check OK - Total orders: {stats['totalOrders']}")
    except Exception as e:
        logger.error(f"Health check failed: {e}")


def main():
    """Main scheduler entry point"""
    sync_interval = int(os.getenv("SYNC_INTERVAL_MINUTES", "5"))
    
    scheduler = BlockingScheduler()
    
    # Google Sheets sync every N minutes
    if os.getenv("GOOGLE_SHEETS_SPREADSHEET_ID"):
        scheduler.add_job(
            sync_google_sheets,
            IntervalTrigger(minutes=sync_interval),
            id='google_sheets_sync',
            name='Sync orders from Google Sheets'
        )
        logger.info(f"Scheduled Google Sheets sync every {sync_interval} minutes")
    
    # Facebook sync every N minutes
    if os.getenv("FACEBOOK_ACCESS_TOKEN"):
        scheduler.add_job(
            sync_facebook,
            IntervalTrigger(minutes=sync_interval),
            id='facebook_sync',
            name='Sync orders from Facebook'
        )
        logger.info(f"Scheduled Facebook sync every {sync_interval} minutes")
    
    # Export to Google Sheets every hour
    if os.getenv("GOOGLE_SHEETS_SPREADSHEET_ID"):
        scheduler.add_job(
            export_to_sheets,
            IntervalTrigger(hours=1),
            id='export_to_sheets',
            name='Export orders to Google Sheets'
        )
        logger.info("Scheduled export to Google Sheets every hour")
    
    # Health check every 5 minutes
    scheduler.add_job(
        health_check,
        IntervalTrigger(minutes=5),
        id='health_check',
        name='API Health Check'
    )
    
    logger.info("Starting Wassel integration scheduler...")
    
    try:
        scheduler.start()
    except (KeyboardInterrupt, SystemExit):
        logger.info("Scheduler stopped")


if __name__ == "__main__":
    main()
