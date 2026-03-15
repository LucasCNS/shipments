-- Initialize PostgreSQL database for Costs application
-- This script is run automatically when the PostgreSQL container starts

-- Create the costs database if it doesn't exist
SELECT 'CREATE DATABASE costs'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'costs');

-- Log
SELECT 'Costs database initialization script executed at ' || CURRENT_TIMESTAMP;
