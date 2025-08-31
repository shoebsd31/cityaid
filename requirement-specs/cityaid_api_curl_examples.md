
# CityAid API — curl examples

> Prereq: obtain an OAuth2 access token from Microsoft Entra ID and export it:
>
> ```bash
> export TOKEN="eyJhbGciOi..."
> export BASE="https://api.cityaid.example.com"
> export CASE="CS-2025-PUN-AL-001"
> ```

## List cases
```bash
curl -s -H "Authorization: Bearer $TOKEN" "$BASE/cases?city=PUN&team=AL&page=1&pageSize=50"
```

## Create case
```bash
curl -s -X POST "$BASE/cases" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{
  "city": "PUN",
  "team": "AL",
  "title": "Medical equipment grant",
  "description": "Beds & consumables"
}'
```

## Get / Update case
```bash
curl -s -H "Authorization: Bearer $TOKEN" "$BASE/cases/$CASE"

curl -s -X PATCH "$BASE/cases/$CASE" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{
  "title": "Medical equipment grant - Phase 1"
}'
```

## Files: attach metadata, list
```bash
curl -s -X POST "$BASE/cases/$CASE/files" -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{
  "name": "blooddrive_poster.pdf",
  "sharePointUrl": "https://contoso.sharepoint.com/sites/Pune-Alpha/Analysis/blooddrive_poster.pdf",
  "sensitivity": "Low"
}'

curl -s -H "Authorization: Bearer $TOKEN" "$BASE/cases/$CASE/files"
```

## Approvals
```bash
curl -s -X POST -H "Authorization: Bearer $TOKEN" "$BASE/cases/$CASE/submit"
curl -s -X POST -H "Authorization: Bearer $TOKEN" "$BASE/cases/$CASE/approve"
curl -s -X POST -H "Authorization: Bearer $TOKEN" "$BASE/cases/$CASE/reject"
curl -s -X POST -H "Authorization: Bearer $TOKEN" "$BASE/cases/$CASE/retrigger"
```
