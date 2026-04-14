# Mzansi Build
**A Platform for Collaborative Build-in-Public Development**

Mzansi Build is a centralized platform designed for developers to showcase active projects, track progress through structured roadmaps, and facilitate collaboration requests within a community-driven environment.

---

## Live Access
To facilitate an easy demonstration of the system's functionality and architectural integration, the application has been deployed using Microsoft Azure services.

* **Web Application URL:** having an unexpected problem deploying.... 
* **API Swagger Documentation:** [[Swagger UI for the azure hosted API](https://mzansi-builds-api-e2g9fhhmbjf8fgaq.southafricanorth-01.azurewebsites.net/swagger/index.html)]

FOR NOW:Run the web locally, it is connected to the api hosted on Azure

---

## Project Vision and Problem Statement
The software development process often occurs in isolation, resulting in missed opportunities for peer review, mentorship, and collaboration. 

**The Solution:** Mzansi Build addresses this by transforming static project links into dynamic, living entries. My implementation strategy utilized a decoupled architecture, featuring an ASP.NET Core 8 Web API and a Blazor Interactive Server frontend to ensure a modern, responsive user experience and a clear separation of concerns.

---

## Completed Requirements
I have successfully implemented the following core functional requirements:

* **User Authentication and Identity:** A robust registration and login system. Each developer has a unique profile associated with their project contributions.
* **Project Lifecycle Management:** Capability for developers to create new project entries containing titles, descriptions, and GitHub repository links.
* **Dynamic Roadmaps:** A nested stage-creation system allowing users to define specific project milestones and explicitly state the type of support required for each.
* **Community Feed:** A real-time overview of all community builds. The feed includes progress indicators, roadmap expansion capabilities, and ownership detection logic to toggle collaboration actions.
* **Progress Tracking:** An interactive profile management system where developers can mark individual stages as complete. This triggers an automatic recalculation of the project's overall progress percentage.
* **Collaboration Handshake:** A functional request system allowing developers to initiate collaboration. Project owners can manage, accept, or decline these requests directly from their dashboard.

---

## Requirements Not Fully Implemented
Due to the constraints of the project timeline, the following features were not fully finalized:

* **The Celebration Wall:** While the logic for reaching 100% project completion is functional, the dedicated "Celebration Wall" page for archived/completed builds was not completed.
* **Real-time Commenting:** Users can currently initiate collaboration via "Hand Raising," but a dedicated free-text commenting thread per project is currently slated for a future iteration.
* **Automated GitHub Invitation:** The backend logic for Octokit (GitHub API) integration was initiated; however, the final automated handshake for repository invitations is not live to ensure security regarding personal access token handling.

---

## Technical Stack
* **Hosting:** Microsoft Azure (App Services)
* **Frontend:** Blazor (Interactive Server Mode)
* **Backend:** ASP.NET Core 8 Web API
* **Database:** Entity Framework Core (SQL Server)
* **Styling:** Custom CSS (Mzansi Theme: Green, White, Black) and Bootstrap 5
* **Icons:** Bootstrap Icons

---

## Lessons Learned and Future Improvements
If provided the opportunity to iterate further on this project, I would focus on the following architectural enhancements:

1.  **Test-Driven Development (TDD):** I would implement a rigorous unit testing suite for the collaboration logic earlier in the cycle. Managing data type friction between GUIDs and strings provided significant debugging challenges that TDD would have mitigated.
2.  **Advanced State Management:** I would implement a centralized state container in Blazor to manage global data, such as notification counts, more efficiently across disparate components.
3.  **Third-Party OAuth:** Rather than a custom Identity implementation, I would utilize GitHub OAuth. This would have streamlined the GitHub integration requirements and improved the user onboarding experience.
4.  **Real-time Communication:** I would integrate SignalR into the Community Feed to allow for instantaneous updates and notifications without requiring manual page refreshes.

---

**Developer:** Andile Shane Likhwa Mpika  
**Submission Date:** 14 April 2026
