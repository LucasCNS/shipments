-- Initialize PostgreSQL database for Shipments application
-- This script is run automatically when the PostgreSQL container starts

-- Create the shipments database if it doesn't exist
SELECT 'CREATE DATABASE shipments'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'shipments');

-- Log
SELECT 'Database initialization script executed at ' || CURRENT_TIMESTAMP;
