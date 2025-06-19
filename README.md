[<img src="https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white" />](https://www.linkedin.com/in/erikshafer/) [<img src="https://img.shields.io/badge/YouTube-FF0000?style=for-the-badge&logo=youtube&logoColor=white" />](https://www.youtube.com/@event-sourcing)

[![blog](https://img.shields.io/badge/blog-event--sourcing.dev-blue)](https://www.event-sourcing.dev/) [![Twitter Follow](https://img.shields.io/twitter/url?label=reach%20me%20%40Faelor&style=social&url=https%3A%2F%2Ftwitter.com%2Ffaelor)](https://twitter.com/faelor)


# 🪄 Ecommerce with the Critter Stack 🛒


## 🤔 What is this repository? <a id='1.0'></a>

This repository's goal is to demonstrate how event-driven concepts and orthogonal patterns don't require convoluted or custom implementations which dampen your team's software development. By leveraging the __Critter Stack__, which consists of [Wolverine](https://wolverinefx.io/) + [Marten](https://martendb.io/) from [JasperFX](https://jasperfx.net/), you can super-charge your .NET development while simplifying your codebase and test cases.

Get ready to unlock success with Event Driven Architectures through the Critter Stack!

### 🛒 Ecommerce <a id='1.1'></a>

The "domain" of this effort is ecommerce, as it provides an assortment of use cases that vary in complexity, ranging from listing the details of a single product to the ever-shifting dynamics of the retail supply chain. 

We will use an assortment of features in the Critter Stack long with using modern .NET conventions to best help you understand how to leverage these techniques in your own software projects.


## 🚧 Roadmap <a id='2.0'></a>

This entire effort is considered a work-in-progress (WIP).

Details are being worked out and will be shared soon.


## 🏞️ Value Streams <a id='3.0'></a>

Value Streams are a core concept in [Team Topologies](https://teamtopologies.com/). To grossly simplify, think departments, divisions, or teams within a company.  That is, *organizing business and technology teams for fast flow.*

These value streams are how the overall .NET solution will be broken down. For example, as of 2025-06-11, there is a `Catalog` and `Supply Chain` solution folders to separate the ~~teams~~ value streams of our imaginary ecommerce business.

### Proposed breakdown of value streams and their modules <a id='3.1'></a>

| Value Stream     | Module       | Wolverine | Marten | event sourced | EF Core | RabbitMQ | Kafka | Misc.               |
|------------------|--------------|-----------|--------|---------------|---------|----------|-------|---------------------|
| 📝 Catalog       | Products     | ✅         | 🔴     | 🔴            | ✅       | TBD      | TBD   | AKA items           |
| 📝 Catalog       | Prices       | ✅         | ✅      | ✅             | 🔴      | TBD      | TBD   |                     |
| 📝 Catalog       | Listings     | ✅         | ✅      | ✅             | 🔴      | TBD      | TBD   |                     |
| 📝 Catalog       | Marketplaces | ✅         | ✅      | ✅             | 🔴      | TBD      | TBD   | AKA channels        |
| 🚚 Supply Chain  | Procurement  | ✅         | ✅      | ✅             | 🔴      | TBD      | TBD   |                     |
| 🚚 Supply Chain  | Inventory    | ✅         | ✅      | ✅             | 🔴      | TBD      | TBD   |                     |
| 🚚 Supply Chain  | Suppliers    | ✅         | TBD    | TBD           | TBD     | TBD      | TBD   | AKA vendors         |
| 📦 Customer      | Orders       | ✅         | ✅      | ✅             | 🔴      | TBD      | TBD   |                     |
| 📦 Customer      | Fulfillment  | ✅         | ✅      | ✅             | 🔴      | TBD      | TBD   |                     |
| 📦 Customer      | Support      | ✅         | ✅      | ✅             | 🔴      | TBD      | TBD   |                     |
| 📦 Customer      | Returns      | ✅         | ✅      | ✅             | 🔴      | TBD      | TBD   |                     |
| 🏪 Retail        | Storefront   | TBD       | TBD    | TBD           | TBD     | TBD      | TBD   | Blazor frontend?    |
| 🏪 Retail        | Cart         | ✅         | ✅      | ✅             | TBD     | TBD      | TBD   |                     |
| 🔬 Data Analysis |              |           |        |               |         |          |       | AI, or something 🔥 |
| 💭 Others TBD    |              |           |        |               |         |          |       |                     |
| 💭 Others TBD    |              |           |        |               |         |          |       |                     |


## 🏫 Resources <a id='9.0'></a>

Blogs, articles, and other resources will be listed here. 🚧

### Tools Used <a id='9.1'></a>

I've been a large fan of [JetBrains](https://www.jetbrains.com/)' suite of Integrated Development Environments (IDEs) for the better part of a decade. That includes their dotnet IDE called [Rider](https://www.jetbrains.com/rider/) which is used to work on this effort.

<img src="https://img.shields.io/badge/Rider-000000?style=for-the-badge&logo=Rider&logoColor=white" alt="jetbrains rider">


## 👷‍♂️ Maintainer <a id='10.0'></a>

Erik "Faelor" Shafer

- linkedin: [in/erikshafer](https://www.linkedin.com/in/erikshafer/)
- blog: [event-sourcing.dev](https://www.event-sourcing.dev)
- youtube: [yt/event-sourcing](https://www.youtube.com/@event-sourcing)
- bluesky: [erikshafer](https://bsky.app/profile/erikshafer.bsky.social)
