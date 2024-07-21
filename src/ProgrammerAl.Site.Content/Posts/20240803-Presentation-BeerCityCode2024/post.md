Title: Beer City Code 2024
Published: 2024/08/03
Tags:

- Conference
- Azure
- Azure Managed Identities
- OpenTelemetry

---


# Slide Decks and Demo Code

If you'd like to see the demo code, and presentation slides, they are hosted on GitHub <a target="_blank" href="https://github.com/ProgrammerAL/Presentations-2024/tree/main/beer-city-code">here</a>.

# The Presentations

I gave 2 talks at <a target="_blank" href="https://www.beercitycode.com/">Beer City Code 2024</a> (each detailed below). One that is an introduction to OpenTelemetry for developers, and the other using Azure Managed Identities to avoid using connection strings.

## OpenTelemetry: More than just a different way to log

OpenTelemetry is a vendor neutral, open standard, to track application metrics, logs, and traces across a distributed set of applications. It's a superset of traditional logging and can be used with or instead of log statements. In this session we’ll review the basics of what OpenTelemetry is, why you should consider using it, and see a demo of using it across backend services.

## Azure Managed Identities: Connect Without Connection Strings

Connection strings, tokens, client secrets, and username/password combos are so easy to use. Almost too easy. But in a world where tokens get leaked on an almost daily basis, you DO NOT want to be the person who leaked it. This is where Azure Managed Identities can help you. You can't leak what you don't have. Lets spend some time learning about Managed Identities and then convert an Azure application from using connection strings to accessing everything through a Managed Identity.


