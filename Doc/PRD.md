# Product Requirements Document (PRD)

**Product:** Centralized E-commerce Order Management Platform (Algeria)  
**Version:** 1.0  
**Date:** April 2026  
**Tech Stack:** .NET Core, Blazor, SQL Server (Azure)

## 1. Product Overview

This platform is a unified order management system (OMS) designed specifically for the Algerian e-commerce market. It centralizes orders from multiple fragmented sources, including Facebook, Instagram, WooCommerce, Shopify, and Google Sheets, into a single high-performance dashboard.

The system focuses on the unique Cash on Delivery (COD) workflow, enabling businesses to manage the entire lifecycle from automatic capture to local delivery confirmation across all 58 wilayas.

## 2. Problem Statement

E-commerce sellers in Algeria currently face significant operational hurdles:

- **Fragmented Data:** Orders are scattered across social media DMs, comments, and web stores.
- **Manual Bottlenecks:** Confirming orders via phone and manually entering data into delivery spreadsheets is slow and error-prone.
- **Delivery Gaps:** Lack of real-time integration with local delivery partners makes tracking and status updates a manual task.
- **No Central Truth:** Businesses lack a single dashboard to see their real conversion and delivery rates across all sales channels.

## 3. Goals & Objectives

- **Centralize Operations:** Eliminate the need to switch between apps by capturing 100% of orders via API and webhooks.
- **Optimize COD Workflow:** Provide a dedicated Confirmation interface to move orders from Pending to Confirmed via built-in communication tools.
- **Automate Last-Mile:** Integrated local delivery tracking for all 58 wilayas to reduce Return to Shipper (RTS) rates.
- **Empower Non-Techies:** Offer no-code landing pages and mini-stores for quick deployment.

## 4. MVP Scope (Phase 1)

The first release should stay narrow and prove the core workflow before any integrations are added.

- **Landing Page:** A public product page that explains the platform and pushes users into the demo experience.
- **Simple Login:** A lightweight sign-in screen for confirmation agents and owners.
- **Dashboard Shell:** A dark, operations-style dashboard that matches the screenshot, including search, filters, order rows, and quick status actions.
- **Mock Data First:** Use static or seeded order data so the UI can be validated before integrating live channels.
- **Core Status Flow:** Support the early confirmation journey, such as Pending, Called, Confirmed, Shipped, and Delivered.

## 5. User Roles & Permissions

| Role | Permissions |
| --- | --- |
| Admin/Owner | Full system access: manage integrations, view financial reports, manage inventory, and user settings. |
| Agent (Confirmation) | Access to the Unified Dashboard: call customers, update order statuses, and manage communication logs. |
| Delivery Coordinator | Manage shipment creation, print labels, and track real-time delivery status with partners. |

## 6. Functional Requirements

### 5.1 Multi-Channel Order Capture

- **Automatic Capture:** Integrate Meta Graph API (Facebook/Instagram) and webhooks (WooCommerce/Shopify) to pull orders instantly.
- **Google Sheets Sync:** Real-time two-way sync for sellers still using Sheets as their primary database.
- **Manual Entry:** A rapid-entry form for phone orders or walk-ins.

### 5.2 Unified Dashboard & Status Tracking

- **Status Lifecycle:** Custom-built for Algerian COD: Pending -> Called (No Answer) -> Confirmed -> Shipped -> Delivered/Returned.
- **Real-time Sync:** Blazor-based UI ensures that when an agent updates a status, it reflects across all logged-in instances immediately.
- **Inventory Overview:** Basic stock tracking that updates automatically as orders are confirmed.

### 5.3 Integrated Local Delivery (The 58 Wilayas Module)

- **Auto-Forwarding:** One-click shipment creation with partners like Yalidine or Nord et Sud.
- **Live Tracking:** Pull real-time tracking data from delivery APIs and display it directly on the order detail page.
- **Wilaya/Commune Validation:** Built-in dropdowns for Algerian geography to prevent address errors.

### 5.4 Built-in Communication

- **One-Click Dial/Message:** Direct integration with WhatsApp Web and mobile dialers for rapid order confirmation.
- **Follow-up Workflows:** Automated reminders for No Answer customers to increase confirmation rates.

## 7. Non-Functional Requirements

- **Performance:** Blazor Server/WebAssembly optimized for < 2s load times on 4G/LTE Algerian networks.
- **Reliability:** 99.9% uptime on Azure, ensuring the dashboard is available during peak evening shopping hours.
- **Usability:** High-contrast dashboard (dark/light mode) designed for high-volume data entry.
- **Localization:** Interface available in French and Arabic; support for DZD (Algerian Dinar).

## 8. Out of Scope (Phase 1)

- Advanced multi-location management for multiple physical warehouses.
- Payment gateway integration (focus is strictly on COD for Phase 1).
- AI-based chatbots for automated DM replies (planned for Phase 2).
- Live integrations with Meta, Shopify, WooCommerce, Google Sheets, and delivery APIs until the MVP UI is approved.

## 9. Success Metrics

- **Time Efficiency:** Reduce average time from Order Received to Shipped by 50%.
- **Reduction in Error:** 95% reduction in manual data entry errors.
- **Scalability:** System capability to handle 10,000+ orders per month per client without performance lag.
- **MVP Validation:** Stakeholders can review the landing page, login flow, and dashboard layout before backend integration begins.

## 10. Tech Stack Implementation Detail

- **Frontend:** Blazor WebAssembly for a rich, app-like experience in the browser.
- **Backend:** .NET 8 Web API for secure, scalable business logic.
- **Database:** SQL Server on Azure with specialized tables for wilaya-based logistics.
- **Integrations:** Azure Functions for handling high-volume webhooks from Shopify/WooCommerce.