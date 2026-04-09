# Wassel - E-commerce Order Management Platform

<p align="center">
  <strong>Centralisez vos commandes e-commerce depuis toutes vos sources dans un tableau de bord unique.</strong>
</p>

## 🎯 À propos

Wassel est une plateforme de gestion de commandes conçue spécifiquement pour le marché algérien du e-commerce. Elle permet de:

- 📱 **Centraliser** les commandes de Facebook, WhatsApp, Instagram, Google Sheets
- ✅ **Confirmer** rapidement via un workflow COD optimisé
- 🚚 **Expédier** avec Yalidine, ZR Express, Maystro, EcoTrack
- 📊 **Analyser** vos performances en temps réel

## 🏗️ Architecture

```
├── Api/                    # .NET 8 Web API
├── Application/            # Business logic layer
├── Domain/                 # Domain entities
├── Infrastructure/         # Database & external services
├── Client/                 # Blazor Server frontend
├── integrations/           # Python scripts for external APIs
└── docker-compose.yml      # Production deployment
```

## 🚀 Démarrage rapide

### Prérequis
- .NET 8 SDK
- SQL Server (ou Docker)
- Node.js (pour les intégrations Python optionnelles)

### Développement

```bash
# 1. Démarrer SQL Server avec Docker
docker-compose -f docker-compose.dev.yml up -d

# 2. Lancer l'API
cd Api
dotnet run

# 3. Lancer le Client (nouveau terminal)
cd Client
dotnet run
```

### Production avec Docker

```bash
docker-compose up -d
```

Accès:
- **Frontend:** http://localhost:5001
- **API:** http://localhost:5000

## 🔐 Comptes de démonstration

| Rôle | Email | Mot de passe |
|------|-------|--------------|
| Admin | admin@wassel.dz | Admin123! |
| Agent | agent@wassel.dz | Agent123! |

## 📦 Fonctionnalités

### Dashboard
- Statistiques en temps réel (commandes, taux de confirmation)
- Filtres par statut, source, wilaya
- Création rapide de commandes

### Gestion des commandes
- Import automatique depuis multiples sources
- Workflow COD: En attente → Confirmé → Expédié → Livré
- Mise à jour de statut en un clic

### Gestion des clients
- Base de données clients
- Historique des commandes
- Informations géographiques (58 wilayas)

### Gestion des produits
- Catalogue produits avec SKU
- Suivi de stock
- Catégorisation

## 🔌 Intégrations

### Sources de commandes
- ✅ Facebook Graph API
- ✅ WhatsApp Business API
- ✅ Google Sheets (import/export)
- ✅ Entrée manuelle

### Transporteurs
- ✅ Yalidine
- ✅ ZR Express
- ✅ Maystro Delivery
- ✅ EcoTrack

## 🗺️ Couverture géographique

Toutes les **58 wilayas** d'Algérie sont supportées avec leurs communes.

## 📚 API Documentation

### Authentification
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@wassel.dz",
  "password": "Admin123!"
}
```

### Commandes
```http
GET /api/orders
Authorization: Bearer <token>
```

### Statistiques
```http
GET /api/dashboard/stats
Authorization: Bearer <token>
```

## 🛠️ Configuration des intégrations

1. Copier `integrations/.env.example` vers `integrations/.env`
2. Configurer les clés API:
   - `GOOGLE_SHEETS_CREDENTIALS_PATH`
   - `FACEBOOK_ACCESS_TOKEN`
   - `WHATSAPP_ACCESS_TOKEN`

3. Lancer le scheduler:
```bash
cd integrations
pip install -r requirements.txt
python scheduler.py
```

## 📝 Licence

Propriétaire - Tous droits réservés

---

<p align="center">
  Développé pour le marché e-commerce algérien 🇩🇿
</p>
