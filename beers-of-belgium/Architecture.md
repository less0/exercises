# Beers of Belgium Architecture

## Introduction and Goals

The goal of the project is, to design a system that allows breweries, wholesale sellers and clients to interact to eventually sell beer to the clients.

[Original Task](https://www.reddit.com/r/csharp/comments/165y1da/project_challenge_submitted_by_a_real_company_for/)

## Constraints

This is an exercise project that has no dependencies on any legacy system. Furthermore there are no monetary or time constraints. It's done when it's done. Or maybe never.

## Context & Scope

The application shall be run in a browser. For that exercise making it accessible for the local machine is sufficient.

### Business context

#### General use-cases

```plantuml
left to right direction

actor User

package "Beers of Belgium" {
    usecase "Register as Seller" as registerasseller
    usecase "Register as Brewer" as registerasbrewer
    usecase "Register as Client" as registerasclient
}

User --> registerasseller
User --> registerasbrewer
User --> registerasclient

```

#### Brewer use-cases

```plantuml
left to right direction

actor Brewer

package "Beers of Belgium" {
    usecase "Add Beer" as addbeer
    usecase "Delete Beer" as deletebeer
    usecase "Update Beer" as updatebeer
    usecase "Update Brewery" as updatebrewery
}

Brewer --> addbeer
Brewer --> updatebeer
Brewer --> deletebeer
Brewer --> updatebrewery
```

#### Seller use-cases

```plantuml
left to right direction

actor Seller

package "Beers of Belgium" {
    usecase "Update Wholesale" as updatewholesale
    usecase "Add Beer to Inventory" as addtoinventory
}

Seller --> updatewholesale
Seller --> addtoinventory
```

#### Client use-cases

```plantuml
left to right direction

actor Client

package "Beers of Belgium" {
    usecase "Request Quote" as requestquote
}

Client --> requestquote
```

#### Overview

| Neighbor | Description                                                                                       |
|----------|---------------------------------------------------------------------------------------------------|
| User     | A user that is not associated with a role is only able to register with one of the existing roles |
| Brewers  | Manages their brewery, including the selection of beers the brewery produces.                     |
| Sellers  | Manages their wholesale, especially the inventory that allows requesting quotes                   |
| Clients  | Are only able to request quotes to purchase beers.                                                |

## Solution Strategy

There is a clear separation in domains (Breweries, Wholesale Inventory, Wholesale Billing) that renders a microservice-approach promising, therefor the application shall be implemented using microservices.

The frontend must be implemented to be run in a browser. Because I recently have gained some experience in Angular which I'd like to expand the frontend shall be implemented using Angular. 

As a mediator between the frontend and the microservices, a backend-for-frontend must be implemented. See the subsequent section for the communication between the components.

## Macro-Architecture

### Components

#### `bob-frontend`

The frontend of the application that is implemented to be run in a browser. 

#### `bob-backend`

Since the application is implemented using microservices for the logic, this component provides the main entry point to the application for the frontend. The backend provides a REST-API.

#### `bob-usermanagement`

This component is responsible for managing users and roles. It provides its services to the `bob-backend` synchronously via HTTP.  

#### `bob-brewery` 

This components is responsible for managing breweries and their beers.

#### `bob-wholesale`

This component is responsible for managing wholesales.

#### `bob-wholesale-inventory`

This component is responsible for managing the inventories of wholesale sellers.

#### `bob-wholesale-billing`

This component is responsible for managing the billing of the wholesale sellers.

### Communication

The `bob-frontend` communicates synchronously via a REST-API with the `bob-backend`, as well as the `bob-backend` with the `bob-usermanagement`. 

Communication between the `bob-backend` and the logic services (`bob-brewery`, `bob-wholesale`, `bob-wholesale-inventory`, `bob-wholesale-billing`) is conducted asynchronously via RabbitMQ. Distributed transactions are coordinated by the `bob-backend`.

The REST-API definition and the RabbitMQ messages will be described in an auxiliary document.

### Persistence

Each of the services has its own instance of a SQL Server Express 2022 database running in its own container. Furthermore the backend has its own instance of SQL Server Express 2022 for storing the distributed transactions.

### Application Structure

```plantuml
component [bob-frontend] as frontend
component [bob-backend] as backend
component [bob-usermanagement] as usermanagement
component [bob-brewery] as brewery
component [bob-wholesale] as wholesale
component [bob-wholesale-inventory] as inventory
component [bob-wholesale-billing] as billing
component [rabbitmq]

database backenddb
database usersdb
database brewerydb
database wholesaledb
database inventorydb
database billingdb

Browser --> frontend
frontend --> backend : Uses a REST-API
backenddb <- backend
backend -> usermanagement
usermanagement --> usersdb
backend <--> rabbitmq
rabbitmq <--> brewery
rabbitmq <--> wholesale
rabbitmq <--> inventory
rabbitmq <--> billing

brewery --> brewerydb
wholesale --> wholesaledb
inventory --> inventorydb
billing --> billingdb
```

### Deployment 

The application is deployed using `docker compose`. The container names correspond to the application components mentioned above.

```plantuml
() HTTP as http1
() HTTP as http2

package Application {
    node frontend [
        bob-frontend

        Ports: ""80:80""
    ]

    node backend [
        bob-backend

        Ports ""8080:80""
    ]

    node backenddb [
        backenddb
        ""mssql/server:2022-latest""

        Ports: ""1433""
    ]

    node usermanagement [
        bob-usermanagement

        Ports: ""80""
    ]

    node usersdb [
        usersdb
        ""mssql/server:2022-latest""

        Ports: ""1433""
    ]

    node rabbitmq [
        rabbitmq

        Ports: ""5672""
    ]

    node brewery [
        bob-brewery
    ]

    node brewerydb [
        brewerydb
        ""mssql/server:2022-latest""

        Ports: ""1433""
    ]

    node wholesale [
        bob-wholesale
    ]

    node wholesaledb [
        wholesaledb
        ""mssql/server:2022-latest""

        Ports: ""1433""
    ]

    node inventory [
        bob-wholesale-inventory
    ]

    node inventorydb [
        inventorydb
        ""mssql/server:2022-latest""

        Ports: ""1433""
    ]

    node billing [
        bob-wholesale-billing
    ]

    node billingdb [
        billingdb
        ""mssql/server:2022-latest""

        Ports: ""1433""
    ]
}

http1 --> frontend : Port 80
http2 --> backend : Port 8080

backenddb <- backend

backend -> usermanagement
usermanagement --> usersdb

backend <--> rabbitmq

rabbitmq <--> brewery
brewery --> brewerydb

rabbitmq <--> wholesale
wholesale --> wholesaledb

rabbitmq <--> inventory
inventory --> inventorydb 

rabbitmq <--> billing
billing --> billingdb

```

### Authentication and Authorization

User authentication is implemented using Auth0. Since for this exercise, a commercial license for Auth0 is too expensive, I opted to implement a separate `bob-usermanagement` service that is responsible for storing users role associations.

There are 3 roles that correspond to the actors in the described use-cases

| Role   | Description                            | Privileges                                                            |
|--------|----------------------------------------|-----------------------------------------------------------------------|
| Client | A client of the page that can buy beer | Request quote<br /> See quotes                                        |
| Brewer | A brewer managing a brewery            | Update brewery info<br />Add beer<br /> Delete beer<br /> Update beer |
| Seller | A seller managing a wholesale          | Update wholesale<br /> Add beer to inventory                          |

For sake of simplicity, wholesales and breweries are associated with exactly one seller or brewer respectively.

A user that has no role associated has to be asked after their login what kind of role they have and are associated with that role in the service. Furthermore a wholesale seller has to name its wholesale and a brewer has to name its brewery, which then is created in the respective service. 

### Monitoring

No monitoring will be implemented for this exercise, because this would be beyond the scope. 

### Logging

No dedicated logging solution will be implemented for this exercise, because this would be beyond the scope.