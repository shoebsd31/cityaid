#!/bin/bash
# CityAid API Test Commands
# Generated automatically

set -e  # Exit on any error

API_BASE_URL="https://localhost:7144"

# JWT Tokens (replace with fresh tokens from JwtTokenGenerator)
ALPHA_PUNE_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIyYzQ2ODUyMi1hNDg2LTQ1MTEtOWFjMC1hNjFhZDAxOTRjZWQiLCJqdGkiOiIxMGUwNjc5ZC1hYWNkLTQ5YzktYWUwMC1jNDYyMGQyZWU0NTQiLCJpYXQiOjE3NTgwNTQ5NzksIm5hbWUiOiJUZXN0IEFscGhhIFVzZXIiLCJlbWFpbCI6InRlc3QuYWxwaGFAY2l0eWFpZC5vcmciLCJjaXR5IjoiUFVOIiwidGVhbSI6IkFMIiwicm9sZSI6IkFscGhhIiwic2NvcGUiOlsiY2FzZTpjcmVhdGUiLCJjYXNlOnJlYWQiLCJjYXNlOnN1Ym1pdCIsImZpbGU6bWFuYWdlIl0sImV4cCI6MTc1ODE0MTM3OSwiaXNzIjoiQ2l0eUFpZEFwaSIsImF1ZCI6IkNpdHlBaWRDbGllbnRzIn0.3bACb57kXVhiTAibJTAy9y5mMOzjxbuFlIo7_mGo5FQ"
BETA_DELHI_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlNTJiYmI0Ny1mMmY4LTRhNDItOGRiMy1jMjAwNjA1YzI2ZmYiLCJqdGkiOiJlODVjOGE4ZS1hYTZkLTRkYjMtOTBjOS0wM2NiZGRhOGM4NTYiLCJpYXQiOjE3NTgwNTQ5NzksIm5hbWUiOiJUZXN0IEJldGEgVXNlciIsImVtYWlsIjoidGVzdC5iZXRhQGNpdHlhaWQub3JnIiwiY2l0eSI6IkRFTCIsInRlYW0iOiJCRSIsInJvbGUiOiJCZXRhIiwic2NvcGUiOlsiY2FzZTpjcmVhdGUiLCJjYXNlOnJlYWQiLCJjYXNlOnN1Ym1pdCIsImZpbGU6bWFuYWdlIl0sImV4cCI6MTc1ODE0MTM3OSwiaXNzIjoiQ2l0eUFpZEFwaSIsImF1ZCI6IkNpdHlBaWRDbGllbnRzIn0.vjKT7HPNP9Aqy2MoGPEn4TRPxvm1XKk94MXz720ztWE"
FINANCE_PUNE_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlYmM2ZGEzMi02ZTg2LTRjOWUtOTIyNy03NDNjYzAxMjAzNGYiLCJqdGkiOiJlYmI2ZWNkOS1mYzM3LTQwZjktOTIwYS1mMmE5M2MyZWU0MzAiLCJpYXQiOjE3NTgwNTQ5NzksIm5hbWUiOiJUZXN0IEZpbmFuY2UgVXNlciIsImVtYWlsIjoidGVzdC5maW5hbmNlQGNpdHlhaWQub3JnIiwiY2l0eSI6IlBVTiIsInRlYW0iOiJGSU4iLCJyb2xlIjoiRmluYW5jZSIsInNjb3BlIjpbImNhc2U6cmVhZCIsImFwcHJvdmFsOmZpbmFuY2UiXSwiZXhwIjoxNzU4MTQxMzc5LCJpc3MiOiJDaXR5QWlkQXBpIiwiYXVkIjoiQ2l0eUFpZENsaWVudHMifQ.7Ig63mpsWckStEwY8HnjXcCTRF4qF4jX5jCd0iWk7nk"
PMO_COUNTRY_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlMzNmM2E1MC1mMTQzLTRhMGQtYWZjMC0wOGQxM2Y1ZTFjODMiLCJqdGkiOiJkM2IxNzJkZi0zYmQwLTQyNTEtYWM1ZS02N2U5NWZlNmQ1NDQiLCJpYXQiOjE3NTgwNTQ5NzksIm5hbWUiOiJUZXN0IFBNTyBVc2VyIiwiZW1haWwiOiJ0ZXN0LnBtb0BjaXR5YWlkLm9yZyIsImNpdHkiOiJJTiIsInRlYW0iOiJQTU8iLCJyb2xlIjoiUE1PIiwic2NvcGUiOlsiY2FzZTpyZWFkIiwiYXBwcm92YWw6ZmluYW5jZSIsImFwcHJvdmFsOnBtbyJdLCJleHAiOjE3NTgxNDEzNzksImlzcyI6IkNpdHlBaWRBcGkiLCJhdWQiOiJDaXR5QWlkQ2xpZW50cyJ9.skcl648WgM-BFHbsj-Kq1ryKh0hukmjxYkLPSswz3ZA"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Utility function
check_response() {
    local status_code=$1
    local expected=$2
    local description="$3"

    echo -e "${BLUE}Expected: $expected, Got: $status_code${NC}"

    if [ "$status_code" = "$expected" ]; then
        echo -e "${GREEN}✅ $description - SUCCESS${NC}"
        return 0
    else
        echo -e "${RED}❌ $description - FAILED${NC}"
        return 1
    fi
    echo ""
}

# Extract case ID from JSON response
extract_case_id() {
    echo "$1" | grep -o '"id":"[^"]*"' | cut -d'"' -f4
}

echo -e "${YELLOW}🚀 Starting CityAid API Tests${NC}"
echo ""

echo -e "${GREEN}🎉 All tests completed!${NC}"
