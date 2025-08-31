Step 1 — One-Page Product Vision & Scope

CityAid MCP Access & Approvals

Vision: Empower city-level “Alpha/Beta” teams to submit funding requests (cases) and files via SharePoint, enabling transparent, auditable, and secure workflows through Finance/Country‑PMO review using an OpenAPI‑based API + MCP‑enabled tooling.

Target users:

City‑level Alpha, Beta requestors & Analysts

Finance reviewers at city & country level

PMO at country level

Problem: Disjointed processes across SharePoint and SQL, inconsistent RBAC, delayed approvals, no traceable API for automation.

Top outcomes:

Fast, auditable case submission lifecycle (Init → Pending Finance → PMO review → Approved/Rejected)

Strict RBAC scoped by city/team; enforced via API, row‑level security, SP permissions

Unified source of truth: SharePoint files aligned to case metadata, all accessible via MCP tooling

Programmatic access via OpenAPI + OAuth2, enabling integration and observability

Measurable efficiency: average approval time, error rates, unauthorized access attempts

In-scope:

Case submission & lifecycle management

Attach/list files in SharePoint libraries

RBAC at city/team levels across UI/APIs/DB/SP

OpenAPI + Swagger UI (dev only)

API‑based tech stack (.NET Core), Azure SQL with RLS, Entra‑ID auth

MCP‑aware client enforcing identity context

Out-of‑scope:

Front-end UI beyond essential MVP

Non-MCP interfaces

Analytics dashboard (post-MVP)

Success metrics:

Leading: number of cases processed, % automated, avg approval time

Lagging: % compliance with RBAC, number of unauthorized attempts, audit completeness

Constraints:

Case IDs format (e.g. CS‑2025‑<CityCode>‑<Team>‑###)

RBAC roles exactly as defined in brief (Alpha/Beta/PMO/Finance/Analysis)

Use SharePoint sites per city/team as specified

Must expose OpenAPI 3.0 spec + OAuth 2.0 flows, tokens with Entra app role claims

📄 Step 2 — PRD (Outline & Highlights)
Overview

CityAid MCP Access & Approvals is an API‑first system supporting city-level teams submitting aid cases with file attachments, reviewed by Finance and PMO roles under scoped RBAC.

Goals & Non-Goals

Goals: accelerate approvals, enforce RBAC, traceability, integration-ready API, MCP compatibility.
Non-Goals: full SPA UI, cross-country consolidations, analytics (for v1).

Personas & Roles
Role	Scope	Core capabilities
Alpha / Beta	City	Create case, attach files, self‑approve (analysis)
Analysis	City	Review analytic completeness
Finance	City/Country	Approve/reject funding stage
PMO	Country	Final approval, re-trigger flows
System	API, SP, SQL	Enforce row‑level & file‑level access
Use Cases

Submit case + files → move through approval states

Finance reviews → moves to PMO

PMO re‑trigger if rejected

MCP client lists only accessible cases/files per user identity

Functional Requirements

Case entity with lifecycle states: Initiated → Pending_Analysis → Pending_Finance → Pending_PMO → Approved / Rejected

Files linked by CaseID, metadata columns in SP libraries

RBAC enforced in API (claims), Azure SQL RLS, SharePoint unique permissions

Non-Functional

Security: OAuth 2.0 (auth code & client credentials), JWT bearer, enforce least‑privilege per role

Audit logging of all actions (DB + access via SP)

Performance: API P95 <200 ms, support up to 1k cases/day

Reliability: ≥99.9% uptime for API

API Requirements

Expose OpenAPI 3.0 spec with securitySchemes including OAuth2 flows and scopes per role; operations for create/list/get/update case, attach files, approval actions by role, re-trigger path. 


Data Model

Azure SQL tables: Case, File, ApprovalHistory, RoleAssignment, Users, Teams, Cities. RLS policies filter by city code and team.

File Model

SharePoint: site per city-team (e.g. “Pune‑Alpha”), library “Analysis”; metadata columns: CaseID, City, Team, Sensitivity, ApprovalState.

MCP Integration

MCP client passes user identity, resolves allowed city/team contexts, queries API & SharePoint using Graph.

Risks & Assumptions

Assumes consistent CityCode naming across DB / SP / API

SP permissions complexity (needs Graph calls)

Prompt injection risk if MCP mis-handles identity

Release Plan

MVP (v1.0): case CRUD, lifecycle, RBAC, files, API & Swagger dev, RLS.
v1.1: MCP integration, audit dashboards, re-trigger path.

Metrics & Telemetry

API: request counts by endpoint, errors by type, latency histograms

Usage: cases per role per day, approval durations

Security: failed authZ attempts, access logs

🔐 Step 3 — RBAC Model & Permission Matrix
Principle & Best Practices

Apply least‑privilege, custom roles scoped to resource groups/databases, use groups, JIT via PIM, review assignments regularly 


API (OAuth2 scopes & claims)

scopes: case:create, case:read, case:submit, approval:finance, approval:pmo, file:manage

JWT includes claims: city, team, role (Alpha/Beta/Finance/PMO/Analysis)

Azure SQL Row-Level Security (RLS)

Predicate: user_city = SESSION_CONTEXT('city') AND user_team = SESSION_CONTEXT('team')

Policy enforced on SELECT/UPDATE/Delete on Cases and Files

SharePoint File Permissions

Libraries scoped by site per city-team; permissions to groups: e.g. Group_Pune_Alpha_Analysts, Group_Pune_Finance, Group_Pune_PMO.

Users assigned to groups matching city-team-role.

Example Scenarios
Role/User	Allowed Actions	Example Case IDs	Disallowed
Alpha Pune Analyst	create/read own cases/files	CS-2025-PUN-AL-001	can't see CS-2025-PUN-BE-* or other cities
Beta Delhi Analyst	attach files, read own cases	CS-2025-DEL-BE-*	can't view others
Pune Finance	approve finance stage of city cases	CS-2025-PUN-*	no access to Delhi
Country PMO (IN)	view + final approve country cases across cities	CS-2025-PUN-, CS-2025-DEL-	no cross-country outside 'IN'
Permission Matrix (Action × Role)
| Action                  | Alpha | Beta | Analysis | Finance | PMO |
|-------------------------|:-----:|:----:|:--------:|:-------:|:---:|
| Create case             | ✓     | ✓    |          |         |     |
| Read own case           | ✓     | ✓    | ✓        | ✓       | ✓   |
| Attach files to case    | ✓     | ✓    | ✓        |         |     |
| Move to Pending Finance | ✓     | ✓    | ✓        |         |     |
| Finance approve/reject  |       |      |          | ✓       |     |
| PMO final approve/reject|       |      |          |         | ✓   |
| Re-trigger rejected     |       |      |          |         | ✓   |
| Audit logs view         | ✓*    | ✓*   | ✓*       | ✓       | ✓   |


* own scope only