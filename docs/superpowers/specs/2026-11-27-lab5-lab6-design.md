# Lab 5 + Lab 6 Design

## Intent
Complete the supplied GAM106 Lab 5/6 on the existing Game API and a sibling Next.js frontend, then test and document both. Preserve existing Backend behavior and database.

## Goals
- Lab 5: owner-scoped `GET /api/characters`, owner-scoped `DELETE /api/characters/{id}` returning 204/404, and CORS limited to `http://localhost:3000`.
- Lab 6: JavaScript Next.js App Router app, plain CSS, four required routes, centralized `lib/api.js`, registration/login, character list/create/delete/logout, responsive home and image attribution.
- Execute TC-A01..A07 and TC-B01..B13 as far as available services/tools allow; report blocked cases truthfully.
- Provide README, `.env.example`, and completion report without secrets.

## Non-goals and constraints
- No TypeScript, Tailwind, UI library, schema change, migration, Backend rewrite, web character update, or unrelated features.
- Do not delete database/data or user changes. Do not store secrets in Git.
- Preserve existing project and use `D:\AI\feGame` (sibling) unless it exists, in which case inspect and preserve it.
- Origins remain exact: Backend permits `http://localhost:3000`, not `127.0.0.1:3000`.

## Approaches
1. Minimal extension of current Backend plus separate Next.js frontend (chosen): lowest risk; uses existing entities, DTOs, services, JWT and GET implementation.
2. Rewrite Backend and frontend together: rejected because existing Auth/EF ownership code works and rewrite risks regressions.
3. Put frontend inside Backend: rejected because Lab requires separately run Next.js at port 3000 and sibling project is clearer.

## Design calls
- Reuse `CharacterService.GetAllAsync`; add delete service method and controller action. Owner-check in query; missing/foreign both return exact Lab 404 message. Delete returns 204 with no body.
- Add named CORS policy for one origin, required methods/headers, `UseCors` before authentication.
- Lab 6 requires create form to submit name and class, but current backend accepts only name and defaults class. Update request/service contract to accept selected Warrior/Mage/Archer and validate; existing database already has Class, so no migration.
- Next.js pages: `/`, `/dang-nhap`, `/tai-khoan`, `/nguon-anh`; one client API module owns all `fetch` calls and token/localStorage behavior.
- User-provided asset/legal provenance must be verifiable. Prefer locally stored permissively licensed images with attribution; if network/source verification is unavailable, use original CSS artwork and disclose no external images rather than invent attribution.
- Auth secrets are provided locally through the existing secure launcher/environment; never log or persist them.

## Verification
- Backend build, Swagger OpenAPI route inspection, CORS preflight and origin denial, unauthenticated 401. Authenticated DB-dependent ownership/delete cases need usable local MySQL credentials.
- Frontend `npm run lint`, `npm run build`, real browser tests at 1920/1366/390, API flow and failure/401/204 handling when browser/backend/DB are available.
- Final report separates PASS, FAIL and BLOCKED; no fabricated screenshots.
