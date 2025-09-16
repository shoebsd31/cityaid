#!/bin/bash

# CityAid API Sample cURL Commands
# Make sure the API is running at https://localhost:7144

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

API_BASE_URL="https://localhost:7144"

# JWT Tokens (24 hour expiration - regenerate from JwtTokenGenerator if expired)
ALPHA_PUNE_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIyYzQ2ODUyMi1hNDg2LTQ1MTEtOWFjMC1hNjFhZDAxOTRjZWQiLCJqdGkiOiIxMGUwNjc5ZC1hYWNkLTQ5YzktYWUwMC1jNDYyMGQyZWU0NTQiLCJpYXQiOjE3NTgwNTQ5NzksIm5hbWUiOiJUZXN0IEFscGhhIFVzZXIiLCJlbWFpbCI6InRlc3QuYWxwaGFAY2l0eWFpZC5vcmciLCJjaXR5IjoiUFVOIiwidGVhbSI6IkFMIiwicm9sZSI6IkFscGhhIiwic2NvcGUiOlsiY2FzZTpjcmVhdGUiLCJjYXNlOnJlYWQiLCJjYXNlOnN1Ym1pdCIsImZpbGU6bWFuYWdlIl0sImV4cCI6MTc1ODE0MTM3OSwiaXNzIjoiQ2l0eUFpZEFwaSIsImF1ZCI6IkNpdHlBaWRDbGllbnRzIn0.3bACb57kXVhiTAibJTAy9y5mMOzjxbuFlIo7_mGo5FQ"
BETA_DELHI_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlNTJiYmI0Ny1mMmY4LTRhNDItOGRiMy1jMjAwNjA1YzI2ZmYiLCJqdGkiOiJlODVjOGE4ZS1hYTZkLTRkYjMtOTBjOS0wM2NiZGRhOGM4NTYiLCJpYXQiOjE3NTgwNTQ5NzksIm5hbWUiOiJUZXN0IEJldGEgVXNlciIsImVtYWlsIjoidGVzdC5iZXRhQGNpdHlhaWQub3JnIiwiY2l0eSI6IkRFTCIsInRlYW0iOiJCRSIsInJvbGUiOiJCZXRhIiwic2NvcGUiOlsiY2FzZTpjcmVhdGUiLCJjYXNlOnJlYWQiLCJjYXNlOnN1Ym1pdCIsImZpbGU6bWFuYWdlIl0sImV4cCI6MTc1ODE0MTM3OSwiaXNzIjoiQ2l0eUFpZEFwaSIsImF1ZCI6IkNpdHlBaWRDbGllbnRzIn0.vjKT7HPNP9Aqy2MoGPEn4TRPxvm1XKk94MXz720ztWE"
FINANCE_PUNE_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlYmM2ZGEzMi02ZTg2LTRjOWUtOTIyNy03NDNjYzAxMjAzNGYiLCJqdGkiOiJlYmI2ZWNkOS1mYzM3LTQwZjktOTIwYS1mMmE5M2MyZWU0MzAiLCJpYXQiOjE3NTgwNTQ5NzksIm5hbWUiOiJUZXN0IEZpbmFuY2UgVXNlciIsImVtYWlsIjoidGVzdC5maW5hbmNlQGNpdHlhaWQub3JnIiwiY2l0eSI6IlBVTiIsInRlYW0iOiJGSU4iLCJyb2xlIjoiRmluYW5jZSIsInNjb3BlIjpbImNhc2U6cmVhZCIsImFwcHJvdmFsOmZpbmFuY2UiXSwiZXhwIjoxNzU4MTQxMzc5LCJpc3MiOiJDaXR5QWlkQXBpIiwiYXVkIjoiQ2l0eUFpZENsaWVudHMifQ.7Ig63mpsWckStEwY8HnjXcCTRF4qF4jX5jCd0iWk7nk"
PMO_COUNTRY_TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJlMzNmM2E1MC1mMTQzLTRhMGQtYWZjMC0wOGQxM2Y1ZTFjODMiLCJqdGkiOiJkM2IxNzJkZi0zYmQwLTQyNTEtYWM1ZS02N2U5NWZlNmQ1NDQiLCJpYXQiOjE3NTgwNTQ5NzksIm5hbWUiOiJUZXN0IFBNTyBVc2VyIiwiZW1haWwiOiJ0ZXN0LnBtb0BjaXR5YWlkLm9yZyIsImNpdHkiOiJJTiIsInRlYW0iOiJQTU8iLCJyb2xlIjoiUE1PIiwic2NvcGUiOlsiY2FzZTpyZWFkIiwiYXBwcm92YWw6ZmluYW5jZSIsImFwcHJvdmFsOnBtbyJdLCJleHAiOjE3NTgxNDEzNzksImlzcyI6IkNpdHlBaWRBcGkiLCJhdWQiOiJDaXR5QWlkQ2xpZW50cyJ9.skcl648WgM-BFHbsj-Kq1ryKh0hukmjxYkLPSswz3ZA"

echo -e "${YELLOW}🚀 CityAid API Sample Tests${NC}"
echo -e "${BLUE}API Base URL: $API_BASE_URL${NC}"
echo ""

# Utility function to check responses
check_response() {
    local response="$1"
    local expected_status="$2"
    local description="$3"

    # Extract status code (last 3 characters)
    local status_code=$(echo "$response" | tail -c 4)
    # Extract body (everything except last 3 characters)
    local body=$(echo "$response" | head -c -4)

    echo -e "${BLUE}Expected: $expected_status, Got: $status_code${NC}"

    if [ "$status_code" = "$expected_status" ]; then
        echo -e "${GREEN}✅ $description - SUCCESS${NC}"
        echo -e "${GREEN}Response: $body${NC}"
        return 0
    else
        echo -e "${RED}❌ $description - FAILED${NC}"
        echo -e "${RED}Response: $body${NC}"
        return 1
    fi
    echo ""
}

# Extract case ID from JSON response
extract_case_id() {
    echo "$1" | grep -o '"id":"[^"]*"' | cut -d'"' -f4
}

echo -e "${YELLOW}=== CASE CREATION TESTS ===${NC}"

# 1. Create Alpha Healthcare Case - Pune Blood Donation Drive
echo -e "${YELLOW}1. Creating Alpha Healthcare Case - Pune Blood Donation Drive${NC}"
response_alpha_pune=$(curl -X POST "$API_BASE_URL/cases" \
  -H "Authorization: Bearer $ALPHA_PUNE_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "city": "PUN",
    "team": "AL",
    "title": "Blood Donation Drive - Pune Medical College",
    "description": "Organize a blood donation camp at Pune Medical College to support emergency blood requirements. Target: 200 units of blood. Duration: 2 days."
  }' \
  -w "%{http_code}" -s)

status_alpha_pune=$(echo "$response_alpha_pune" | tail -c 4)
body_alpha_pune=$(echo "$response_alpha_pune" | head -c -4)
case_id_alpha_pune=$(extract_case_id "$body_alpha_pune")
echo "Created Case ID: $case_id_alpha_pune"
check_response "$response_alpha_pune" "201" "Alpha Pune Case Creation"

sleep 2

# 2. Create Beta Education Case - Delhi Autism School Admission
echo -e "${YELLOW}2. Creating Beta Education Case - Delhi Autism School Admission${NC}"
response_beta_delhi=$(curl -X POST "$API_BASE_URL/cases" \
  -H "Authorization: Bearer $BETA_DELHI_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "city": "DEL",
    "team": "BE",
    "title": "Autism School Admission Support - Delhi",
    "description": "Help 50 autistic children get admission in specialized schools across Delhi. Includes application support and documentation assistance."
  }' \
  -w "%{http_code}" -s)

status_beta_delhi=$(echo "$response_beta_delhi" | tail -c 4)
body_beta_delhi=$(echo "$response_beta_delhi" | head -c -4)
case_id_beta_delhi=$(extract_case_id "$body_beta_delhi")
echo "Created Case ID: $case_id_beta_delhi"
check_response "$response_beta_delhi" "201" "Beta Delhi Case Creation"

sleep 2

# 3. Create Alpha Healthcare Case - Mumbai Laser Treatment
echo -e "${YELLOW}3. Creating Alpha Healthcare Case - Mumbai Laser Treatment${NC}"
response_alpha_mumbai=$(curl -X POST "$API_BASE_URL/cases" \
  -H "Authorization: Bearer $ALPHA_PUNE_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "city": "MUM",
    "team": "AL",
    "title": "Laser Treatment Equipment - Mumbai Remote Areas",
    "description": "Provide laser treatment equipment for eye surgeries in remote areas of Mumbai. Equipment includes portable laser units and trained technicians."
  }' \
  -w "%{http_code}" -s)

check_response "$response_alpha_mumbai" "201" "Alpha Mumbai Case Creation"

sleep 2

echo -e "${YELLOW}=== CASE LISTING TESTS ===${NC}"

# 4. List All Cases - PMO Country View
echo -e "${YELLOW}4. PMO User - List All Cases Across Country${NC}"
response_pmo_list=$(curl -X GET "$API_BASE_URL/cases?page=1&pageSize=10" \
  -H "Authorization: Bearer $PMO_COUNTRY_TOKEN" \
  -H "Content-Type: application/json" \
  -w "%{http_code}" -s)

check_response "$response_pmo_list" "200" "PMO Country Cases List"

sleep 2

# 5. List Pune Cases - Finance User
echo -e "${YELLOW}5. Finance User - List Pune Cases Only${NC}"
response_finance_list=$(curl -X GET "$API_BASE_URL/cases?city=PUN&page=1&pageSize=10" \
  -H "Authorization: Bearer $FINANCE_PUNE_TOKEN" \
  -H "Content-Type: application/json" \
  -w "%{http_code}" -s)

check_response "$response_finance_list" "200" "Finance Pune Cases List"

sleep 2

# 6. List Alpha Cases - Alpha User
echo -e "${YELLOW}6. Alpha User - List Alpha Team Cases${NC}"
response_alpha_list=$(curl -X GET "$API_BASE_URL/cases?team=AL" \
  -H "Authorization: Bearer $ALPHA_PUNE_TOKEN" \
  -H "Content-Type: application/json" \
  -w "%{http_code}" -s)

check_response "$response_alpha_list" "200" "Alpha Team Cases List"

sleep 2

echo -e "${YELLOW}=== WORKFLOW TESTS ===${NC}"

# Only run workflow tests if we have a valid case ID
if [ -n "$case_id_alpha_pune" ]; then
    # 7. Submit Case for Approval
    echo -e "${YELLOW}7. Submit Alpha Pune Case for Approval${NC}"
    response_submit=$(curl -X POST "$API_BASE_URL/cases/$case_id_alpha_pune/submit" \
      -H "Authorization: Bearer $ALPHA_PUNE_TOKEN" \
      -H "Content-Type: application/json" \
      -w "%{http_code}" -s)

    check_response "$response_submit" "200" "Case Submission"

    sleep 2

    # 8. Finance Approve Case
    echo -e "${YELLOW}8. Finance User Approves Case${NC}"
    response_finance_approve=$(curl -X POST "$API_BASE_URL/cases/$case_id_alpha_pune/approve" \
      -H "Authorization: Bearer $FINANCE_PUNE_TOKEN" \
      -H "Content-Type: application/json" \
      -w "%{http_code}" -s)

    check_response "$response_finance_approve" "200" "Finance Approval"

    sleep 2

    # 9. PMO Final Approval
    echo -e "${YELLOW}9. PMO User Final Approval${NC}"
    response_pmo_approve=$(curl -X POST "$API_BASE_URL/cases/$case_id_alpha_pune/approve" \
      -H "Authorization: Bearer $PMO_COUNTRY_TOKEN" \
      -H "Content-Type: application/json" \
      -w "%{http_code}" -s)

    check_response "$response_pmo_approve" "200" "PMO Final Approval"

    sleep 2

    # 10. Attach File to Case
    echo -e "${YELLOW}10. Attach File to Case${NC}"
    response_attach_file=$(curl -X POST "$API_BASE_URL/cases/$case_id_alpha_pune/files" \
      -H "Authorization: Bearer $ALPHA_PUNE_TOKEN" \
      -H "Content-Type: application/json" \
      -d '{
        "name": "Blood_Donation_Proposal.pdf",
        "sharePointUrl": "https://cityaid.sharepoint.com/sites/Alpha/Shared%20Documents/Blood_Donation_Proposal.pdf",
        "sensitivity": "Medium"
      }' \
      -w "%{http_code}" -s)

    check_response "$response_attach_file" "201" "File Attachment"

    sleep 2

    # 11. List Files for Case
    echo -e "${YELLOW}11. List Files for Case${NC}"
    response_list_files=$(curl -X GET "$API_BASE_URL/cases/$case_id_alpha_pune/files" \
      -H "Authorization: Bearer $PMO_COUNTRY_TOKEN" \
      -H "Content-Type: application/json" \
      -w "%{http_code}" -s)

    check_response "$response_list_files" "200" "List Case Files"
else
    echo -e "${RED}❌ Skipping workflow tests - no valid case ID${NC}"
fi

echo ""
echo -e "${GREEN}🎉 API Testing Complete!${NC}"
echo -e "${BLUE}Created Cases:${NC}"
[ -n "$case_id_alpha_pune" ] && echo -e "  ${GREEN}Alpha Pune:${NC} $case_id_alpha_pune"
[ -n "$case_id_beta_delhi" ] && echo -e "  ${GREEN}Beta Delhi:${NC} $case_id_beta_delhi"
echo ""
echo -e "${YELLOW}💡 Tip: Use Swagger UI at $API_BASE_URL/swagger for interactive testing${NC}"