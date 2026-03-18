Title: GLUGNET Meetup March 2026
Published: 2026/03/19
Tags:

- Meetup
- C#
- Video

Presentations:
- Id: 1
  SlidesRootUrl: https://raw.githubusercontent.com/ProgrammerAL/Presentations-2026/main/glugnet-meetup-march-2026

---

## GLUGNET March 2026

The Greater Lansing User Group for .NET Developers (aka GLUGNET) meetup group invited me to speak virtually at their March 2026 meetup on "Avoiding null! in C#": https://www.meetup.com/glugnet/events/313635890

## Avoiding null! in C#

<div class="post-multiple-links-div">
  <a class="post-session-content-link" target="_blank" href="https://github.com/ProgrammerAL/Presentations-2026/tree/main/glugnet-meetup-march-2026">View Session Content on GitHub</a>
  <a class="post-view-session-content-link" href="/posts/20260319_GLUGNETMarch2026/slides/1">View Slides in Browser</a>
</div>

__Session Abstract__: 
Null references are a problem that can show up anywhere. As developers, we have to litter out code with null checks.

Thankfully C# has a feature to help with that, Nullable Reference Types (NRTs). They utilize a mix of syntax and special C# Attributes to enable null checking at compile time. It's not a perfect system, but it's WAY better than not using NRTs.

In this session we'll review (and demo!) the different ways Nullable Reference Types help us when something may be null, by talking about:
- The helpful NRT C# syntax and attributes
- How NRT code is different for object deserialization
- Cases where NRTs aren't as helpful as we'd like them to be

