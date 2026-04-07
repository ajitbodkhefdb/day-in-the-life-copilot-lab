# PRD: Add Course Prerequisites

**Branch:** `feature/add-course-prerequisites`
**Related Issues:** #N/A
**Date:** 2026-04-07
**Status:** Draft

---

## 1. Feature Overview

This feature introduces a **course prerequisites** system to ContosoUniversity, allowing administrators to define which courses a student must have previously completed (or be enrolled in) before enrolling in a given course. The system enforces prerequisite rules at enrollment time and surfaces prerequisite information on course detail pages.

---

## 2. User Stories

1. **As an administrator**, I want to define one or more prerequisite courses for a course, so that students are guided toward the correct learning sequence.

2. **As a student**, I want to see the prerequisites for a course before enrolling, so that I know which courses I need to complete first.

3. **As a student**, I want to be prevented from enrolling in a course whose prerequisites I have not met, so that I am not placed in courses I am unprepared for.

---

## 3. Acceptance Criteria

### Story 1 — Admin defines prerequisites
- Admin can add/remove prerequisite course(s) on the Course Create and Edit pages.
- Prerequisites are saved to the database and persisted across sessions.
- A course cannot be set as its own prerequisite (circular self-reference guard).
- Admin receives a success or validation error message after saving.

### Story 2 — Student views prerequisites
- The Course Details page lists all prerequisite courses by title and course number.
- The Course Index page optionally shows a "Has Prerequisites" indicator.

### Story 3 — Prerequisite enforcement at enrollment
- On enrollment, the system checks whether the student has a completed enrollment for each prerequisite course.
- If prerequisites are not met, enrollment is rejected with a clear error message listing unmet prerequisites.
- If no prerequisites are defined, enrollment proceeds as today (no regression).

---

## 4. Technical Considerations

### Affected Projects
| Project | Changes |
|---|---|
| `ContosoUniversity.Core` | New `CoursePrerequisite` model; update `Course` model with `Prerequisites` and `RequiredBy` navigation properties |
| `ContosoUniversity.Infrastructure` | EF Core migration; update `SchoolContext`; seed data in `DbInitializer` |
| `ContosoUniversity.Web` | Update `CoursesController` (Create/Edit/Details); update `CourseViewModel`; update Views |
| `ContosoUniversity.Tests` | Unit + integration tests for prerequisite logic and controller actions |
| `ContosoUniversity.PlaywrightTests` | E2E tests for admin prerequisite management and student enrollment flow |

### Database Changes (EF Core)
- Add `CoursePrerequisite` join table with columns: `CourseID` (FK → Course), `PrerequisiteCourseID` (FK → Course).
- Many-to-many self-referencing relationship on `Course`.
- New EF migration: `AddCoursePrerequisites`.

```csharp
// ContosoUniversity.Core/Models/CoursePrerequisite.cs
public class CoursePrerequisite
{
    public int CourseID { get; init; }
    public int PrerequisiteCourseID { get; init; }
    public virtual Course Course { get; init; } = null!;
    public virtual Course PrerequisiteCourse { get; init; } = null!;
}
```

```csharp
// Course.cs additions
public virtual ICollection<CoursePrerequisite> Prerequisites { get; set; } = new List<CoursePrerequisite>();
public virtual ICollection<CoursePrerequisite> RequiredBy { get; set; } = new List<CoursePrerequisite>();
```

### SchoolContext Updates
- Register `DbSet<CoursePrerequisite>` and configure composite key `(CourseID, PrerequisiteCourseID)` and both FK relationships in `OnModelCreating`.

### API / Controller Changes
- `CoursesController.Create` (GET/POST): populate and bind selected prerequisite course IDs via `ViewData["PrerequisiteCourses"]`.
- `CoursesController.Edit` (GET/POST): same as Create; diff existing prerequisites on save.
- `CoursesController.Details` (GET): include `.Prerequisites` in query via `IRepository<Course>`.
- New prerequisite-validation method (or domain service): `ValidatePrerequisites(Student, Course)` — returns unmet prerequisites.
- `StudentsController` or enrollment endpoint: call prerequisite validation before saving enrollment.

### Views
- `Views/Courses/Create.cshtml` / `Edit.cshtml`: multi-select list for prerequisite courses.
- `Views/Courses/Details.cshtml`: prerequisites section listing linked course titles.
- `Views/Courses/Index.cshtml`: optional badge/indicator for courses with prerequisites.

---

## 5. Testing Requirements

### Unit Tests (`ContosoUniversity.Tests`)
- `ValidatePrerequisites_StudentHasCompletedAllPrereqs_ReturnsEmpty`
- `ValidatePrerequisites_StudentMissingPrereq_ReturnsMissingCourses`
- `ValidatePrerequisites_CourseHasNoPrereqs_ReturnsEmpty`
- `CoursesController_Edit_SavesPrerequisitesCorrectly`
- Follow `MethodName_Condition_ExpectedResult` naming (per `dotnet.instructions.md`).

### Integration Tests (`ContosoUniversity.Tests/Integration`)
- Use `WebApplicationFactory<Program>` (pattern already in `ContosoUniversity.Tests/Integration`).
- Test that POST to `/Courses/Edit` persists prerequisites to the in-memory/SQLite DB.
- Test that enrollment endpoint rejects request when prerequisite not met (HTTP 400 / redirect with error).

### E2E Tests (`ContosoUniversity.PlaywrightTests`)
- Admin flow: navigate to Course Edit → assign a prerequisite → save → verify on Details page.
- Student flow: attempt to enroll in a course with unmet prerequisites → verify error message.
- Use `data-testid` selectors on new UI elements.

---

## 6. Out of Scope

- Graded prerequisite thresholds (e.g., "must have passed with grade B or higher").
- Transitive/recursive prerequisite chains (e.g., A requires B requires C — only direct prerequisites enforced).
- Student-facing prerequisite waiver requests.
- Automated prerequisite suggestions via ML.

---

## 7. Dependencies

- Existing `Enrollment` model (links `Student` ↔ `Course` with optional `Grade`) — used for prerequisite-completion check.
- Existing `IRepository<T>` pattern in `ContosoUniversity.Core/Interfaces/IRepository.cs`.
- EF Core migrations infrastructure already in place (`ContosoUniversity.Infrastructure/Data/`).
- `[Authorize(Roles = "Admin")]` guard already applied to Course Create/Edit — prerequisite management inherits this.
