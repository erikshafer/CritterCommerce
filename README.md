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

WIP of the WIP. :)

## 🏞️ Value Streams <a id='3.0'></a>

Value Streams are a core concept in [Team Topologies](https://teamtopologies.com/). To grossly simplify, think departments, divisions, or teams within a company.  That is, *organizing business and technology teams for fast flow.*

These value streams are how the overall .NET solution will be broken down. For example, as of 2025-06-11, there is a `Catalog` and `Supply Chain` solution folders to separate the ~~teams~~ value streams of our imaginary ecommerce business.

### 🏞️ Value streams and their responsibilities (chart) <a id='3.1'></a>

| Value Stream                | Responsibility                                       |
|-----------------------------|------------------------------------------------------|
| 📝 **Catalog**              | Product definitions, SKUs, attributes                |
| 📺 **Channels**             | Marketplace listings, syncs, channel-specific logic  |
| 📨 **Orders**               | Customer purchases, order lifecycle                  |
| 🏪 **Checkout**             | Shopping cart, validation, checkout flow             |
| 📦 **Inventory**            | Stock levels, reservations, fulfillment readiness    |
| 📒 **Procurement**          | Supply chain, purchase orders, restocking            |
| 🛤️ **Fulfillment**         | Shipping, delivery tracking, warehouse orchestration |
| 💳 **Payments**             | Payment gateway, capture, refunds                    |
| ⚠️ **Promotions & Pricing** | Discounts, price changes, campaigns                  |
| 🔎 **Search & Discovery**   | Read-optimized catalog/index views                   |
| 💼 **Vendor Portal**        | B2B portal, performance dashboards and stats         |
| 🧓🏻 **Customer Accounts**  | Identity, profile, registration                      |
| 💁🏻‍♂️ **Support**         | Tickets, complaints, return handling                 |
| 📫 **Notifications**        | Email, SMS, webhooks, system messaging               |

### 🏞️ Modules across the value streams (chart) <a id='3.2'></a>

An example of various proposed modules in this system, highlighting some technologies and techniques that are being used or under proposal.

| Value Stream               | Module(s)                | Wolverine | Marten | EF Core | CRUD or ES | CQRS | Additional Notes                  |
|----------------------------|--------------------------|-----------|--------|---------|------------|------|-----------------------------------|
| 📦 **Inventory**           | Receiving Shipments      | ✅         | ✅      | ⛔       | ES         | ✅    | ...                               |
| 📦 **Inventory**           | Warehouse Stocks         | ✅         | ✅      | ⛔       | ES         | ✅    | ...                               |
| 📝 **Catalog**             | Products                 | ✅         | ✅      | ⛔       | ES         | ✅    | ...                               |
| 📝 **Catalog**             | Taxonomy                 | ✅         | ✅      | ⛔       | CRUD       | ✅    | Postgres document store           |
| 📺 **Channels**            | Listings                 | ✅         | ✅      | ⛔       | ES         | ✅    | ...                               |
| 📺 **Channels**            | Marketplace Integrations | ✅         | ✅      | ⛔       | ES         | ✅    | ...                               |
| 🏪 **Checkout**            | Storefront               | ...       | ...    | ...     | ...        | ...  | Either frontend or BFF            |
| 🏪 **Checkout**            | Shopping Cart            | ✅         | ✅      | ⛔       | ES         | ✅    | ...                               |
| 📨 **Orders**              | *TBD*                    | ✅         | ✅      | ⛔       | ES         | ✅    | Sagas showcased                   |
| 💳 **Payments**            | *TBD*                    | ✅         | ✅      | ⛔       | ES         | ✅    |                                   |
| 🧓🏻 **Customer Accounts** | *TBD*                    | ✅         | ✅      | ⛔       | ES         | ✅    | Multitenancy showcased            |
| 🧓🏻 **(Legacy) Catalog**  | Catalog                  | ✅         | ⛔      | ✅       | CRUD       | ⛔    | No Critter Stack  at all possibly |
| 🧓🏻 **(Legacy) Catalog**  | Sku Management           | ✅         | ⛔      | ✅       | CRUD       | ⛔    | No Critter Stack  at all possibly |
| 🤔 More TBD                | ...                      | ...       | ...    | ...     | ...        | ...  | ...                               |

## ➡️ Diagrams <a id='4.0'></a>

**Work-in-progress.**

I would like to outline some of the business workflows as well as the technical aspects like architecture.

### Receiving Shipments workflow <a id='4.1'></a>

This diagram visualizes the **ReceivingShipments** workflow and the typical status transitions for a `ReceivedShipment` aggregate. It can be visualized as a simple state machine:

```text
stateDiagram-v2
    [*] --> Created : ReceivedShipmentCreated
    Created --> Receiving : ReceivedShipmentLineItemAdded
    Receiving --> Receiving : ReceivedShipmentLineItemAdded
    Receiving --> Receiving : ReceivedShipmentLineItemQuantityRecorded\n(not all items received)
    Receiving --> Received : ReceivedShipmentLineItemQuantityRecorded\n(all items received)
    Receiving --> Received : ReceivedShipmentMarkedAsReceived
    Received --> PutAway : ReceivedShipmentPutAway
    Created --> [*]
    Receiving --> [*]
    Received --> [*]
    PutAway --> [*]
```

#### **Status Transitions Explained**
- **Created**: Shipment initiated, but no items yet.
- **Receiving**: At least one line item added. Remains here as line items and their received quantities are added.
- **Received**: All line items have been recorded as received (automatically, or via explicit "Mark as Received").
- **PutAway**: The shipment is assigned to a putaway lot/location—no further receiving actions possible.

**Note:**
- Returning to [ * ] just represents an end state; in practice, transitions are unidirectional.

## 🛠️ Local Development <a id='5.0'></a>

WIP. To launch Docker with the `all` profile, use this `docker-compose` command:

```bash
docker-compose --profile all up -d
```

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
