# Wassel Integrations

Scripts Python pour l'intégration avec des services externes.

## Configuration

1. Copier `.env.example` vers `.env`
2. Remplir les clés API nécessaires

## Intégrations disponibles

### Google Sheets
- Import automatique des commandes depuis Google Sheets
- Export des commandes vers Google Sheets

### Facebook/Meta Graph API
- Capture des commandes depuis les conversations de la Page Facebook
- Envoi de confirmations automatiques

### WhatsApp Business API
- Réception des commandes via WhatsApp
- Envoi de confirmations et mises à jour de statut

## Utilisation

```bash
# Installer les dépendances
pip install -r requirements.txt

# Lancer le scheduler pour les syncs automatiques
python scheduler.py

# Ou exécuter manuellement
python google_sheets_sync.py
python facebook_sync.py
```

## Webhooks

Pour Facebook et WhatsApp, configurez les webhooks sur votre serveur:

- Facebook: `POST /webhook/facebook`
- WhatsApp: `POST /webhook/whatsapp`

Le verify token pour WhatsApp doit correspondre à `WHATSAPP_WEBHOOK_VERIFY_TOKEN` dans `.env`.
