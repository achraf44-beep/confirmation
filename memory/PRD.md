# Wassel - Product Requirements Document

## Overview
**Product:** Wassel - Centralized E-commerce Order Management Platform for Algeria  
**Version:** 1.0  
**Date:** January 2026  
**Tech Stack:** .NET 8, Blazor Server, SQL Server, Python (integrations)

## Problem Statement
E-commerce sellers in Algeria face operational challenges:
- Orders scattered across Facebook, WhatsApp, Instagram, Google Sheets
- Manual confirmation processes via phone calls
- No centralized dashboard for COD (Cash on Delivery) operations
- Difficulty integrating with local delivery companies

## Core Requirements

### User Roles
| Role | Permissions |
|------|------------|
| Owner | Full system access |
| Agent | Order confirmation, status updates |
| Delivery Coordinator | Shipment management |

### Order Sources
- [x] Manual entry
- [x] Facebook (integration ready)
- [x] WhatsApp (integration ready)
- [x] Instagram
- [x] Google Sheets (integration ready)
- [x] Shopify
- [x] WooCommerce

### Order Status Workflow
`Pending` → `CalledNoAnswer` → `Confirmed` → `Reprogrammed` → `Shipped` → `Delivered` / `Returned` / `Cancelled`

### Delivery Companies
- [x] Yalidine
- [x] ZR Express
- [x] Maystro Delivery
- [x] EcoTrack

### Geographic Coverage
- All 58 Algerian wilayas included
- Commune-level addressing support

## What's Been Implemented

### Backend (.NET 8 Web API)
- [x] JWT Authentication with role-based authorization
- [x] Orders CRUD with status management
- [x] Customers management
- [x] Products/Inventory management
- [x] Delivery companies management
- [x] Dashboard statistics API
- [x] Algerian wilayas/locations API
- [x] Database seeding with demo data

### Frontend (Blazor Server)
- [x] Landing page with feature presentation
- [x] Login/Register with JWT authentication
- [x] Dashboard with statistics cards
- [x] Orders table with filters (status, source, wilaya)
- [x] Order creation modal
- [x] Status update functionality
- [x] Customers page with CRUD
- [x] Products page with CRUD
- [x] Responsive design (mobile-ready)

### Python Integrations
- [x] Wassel API client library
- [x] Google Sheets sync (import/export)
- [x] Facebook Graph API integration
- [x] WhatsApp Business API integration
- [x] Automated sync scheduler

### Infrastructure
- [x] Docker configuration
- [x] docker-compose for production
- [x] docker-compose.dev for development
- [x] SQL Server configuration

## Demo Credentials
- **Admin:** admin@wassel.dz / Admin123!
- **Agent:** agent@wassel.dz / Agent123!

## API Endpoints

### Authentication
- `POST /api/auth/login` - Login
- `POST /api/auth/register` - Register
- `GET /api/auth/me` - Get current user

### Orders
- `GET /api/orders` - List all orders
- `GET /api/orders/{id}` - Get order details
- `POST /api/orders` - Create order
- `PUT /api/orders/{id}/status` - Update status

### Customers
- `GET /api/customers` - List customers
- `POST /api/customers` - Create customer
- `PUT /api/customers/{id}` - Update customer
- `DELETE /api/customers/{id}` - Delete customer

### Products
- `GET /api/products` - List products
- `POST /api/products` - Create product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Dashboard
- `GET /api/dashboard/stats` - Get statistics

### Locations
- `GET /api/locations/wilayas` - List all 58 wilayas

## Backlog (P1 - High Priority)
- [ ] Real-time order updates via SignalR
- [ ] WhatsApp/Facebook webhook handlers
- [ ] Order detail modal with full history
- [ ] Delivery company API integration (tracking)
- [ ] Print shipping labels

## Backlog (P2 - Medium Priority)
- [ ] Arabic language support
- [ ] Advanced analytics/reports
- [ ] Export to Excel/PDF
- [ ] Customer order history
- [ ] SMS notifications

## Backlog (P3 - Low Priority)
- [ ] AI chatbot for automated replies
- [ ] Multi-store support
- [ ] Payment gateway (future)

## Running the Application

### Development
```bash
# Start SQL Server
docker-compose -f docker-compose.dev.yml up -d

# Run API
cd Api && dotnet run

# Run Client
cd Client && dotnet run
```

### Production (Docker)
```bash
docker-compose up -d
```

Access:
- Client: http://localhost:5001
- API: http://localhost:5000
