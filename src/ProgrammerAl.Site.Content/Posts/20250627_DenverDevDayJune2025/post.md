Title: Denver Dev Day June 2025
Published: 2025/06/27
Tags:

- Conference
- Azure
- C#

Presentations:
- Id: 1
  SlidesRootUrl: https://raw.githubusercontent.com/ProgrammerAL/Presentations-2025/main/denver-dev-day-june-2025/azure-managed-identities
- Id: 2
  SlidesRootUrl: https://raw.githubusercontent.com/ProgrammerAL/Presentations-2025/main/denver-dev-day-june-2025/csharp-pit-of-success

---

## Denver Dev Day June 2025

Denver Dev Day is a twice-yearly conference held in...Denver. It is a one-day software conference for developers. The 2025 June conference was held on June 27th at Red Rocks Community College. https://stirtrek.com/ with the full schedule at https://denverdevday.github.io/jun-2025/sessions.html#sessions

At this conference I performed two sessions. "Azure Managed Identities: Connect Without Connection Strings" and "Setting Up a C# Pit of Success"

## Session: Azure Managed Identities: Connect Without Connection Strings

<div class="post-multiple-links-div">
  <a class="post-session-content-link" target="_blank" href="https://github.com/ProgrammerAL/Presentations-2025/tree/main/denver-dev-day-june-2025/azure-managed-identities">View Session Content on GitHub</a>
  <a class="post-view-session-content-link" href="/posts/20250627_DenverDevDayJune2025/slides/1">View Slides in Browser</a>
</div>

__Session Abstract__: 
While connection strings, tokens, client secrets, and username/password combos are easy to use, using them may be too easy. In a world where tokens get leaked almost daily, you DO NOT want to be the person who leaked it by committing to source control, just posting a picture of yourself online with a password plainly behind you on a post-it note, or some other accident.

Azure Managed Identities change how you authenticate to other Azure services. By abstracting the authentication token away, you and your application never see a token value. Remember, you can't lose what you don't have.

Let's explore Azure Managed Identities and see how easy they are to use in our codebase. We'll also update an application to convert it from using connection strings to using a Managed Identity for all authentications.

## Session: Setting Up a C# Pit of Success

<div class="post-multiple-links-div">
  <a class="post-session-content-link" target="_blank" href="https://github.com/ProgrammerAL/Presentations-2025/tree/main/denver-dev-day-june-2025/csharp-pit-of-success">View Session Content on GitHub</a>
  <a class="post-view-session-content-link" href="/posts/20250627_DenverDevDayJune2025/slides/2">View Slides in Browser</a>
</div>

__Session Abstract__: 
Writing code is only ~40% of software development work. Maintaining high-quality code is maybe another ~20%. Reviewing PRs and updating dependencies are just some of the manual processes we have to deal with that regularly take us away from the fun part of software development: writing code.

Did you know a C# project can tell you when there's a known security vulnerability for a NuGet package it's using? How about using a file to manage warnings/errors/suggestions for the entire project? You can even commit the file to source control, so no one needs a 3rd party extension to manage those settings. (That's the .editorconfig file by the way. It's really cool.)

In this session, we'll look at features like those above (and more!) that we can enable within our codebases to set us up for future success. And best of all, you can enable all of these features for free. So you can enable them whenever you're ready.

