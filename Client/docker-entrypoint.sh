#!/bin/sh

# Use VITE_API_HOST if set, otherwise use default
if [ -z "$VITE_API_HOST" ]; then
    echo "WARNING: VITE_API_HOST is not set, using default: http://localhost:5152"
    export VITE_API_HOST="http://localhost:5152"
fi

# Remove trailing slash if present
VITE_API_HOST=$(echo "$VITE_API_HOST" | sed 's:/*$::')

echo "Using API Host: $VITE_API_HOST"

# Extract just the hostname from the URL for the Host header
BACKEND_HOST=$(echo "$VITE_API_HOST" | sed -E 's|^https?://||' | sed 's|/.*||')
export BACKEND_HOST

echo "Backend Host: $BACKEND_HOST"

# Substitute environment variables in nginx config
envsubst '${VITE_API_HOST} ${BACKEND_HOST}' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf

echo "Generated nginx config:"
cat /etc/nginx/conf.d/default.conf

# Test nginx configuration
nginx -t

# Start nginx
echo "Starting nginx..."
nginx -g "daemon off;"
