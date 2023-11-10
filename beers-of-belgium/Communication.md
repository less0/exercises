# Beers of Belgium Communication

## Introduction

This file is an addendum to the architecture documentation and serves the purpose to define the communication interfaces
between the components of the application. The basic means of communication are described in the architecture document,
this documents describes the exchanges messages in a greater depth.

## REST-API

| URI                               | Method   | Data              | Remarks                                                                                                                                                              |
|-----------------------------------|----------|-------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `/users`                          | `GET`    | `user[]`          | Only returns `user`-objects valid for the authenticated user.                                                                                                        |
| `/users`                          |  `POST`  |  `user`           | The `user.id` is determined server-side. If the role is invalid or the method is called multiply by the same user, an error status is returned.                      | 
| `/breweries`                      | `GET`    | `brewery[]`       | Gets all breweries for the "Client" and "Seller" roles. For the Role "Brewer" this only returns the associated breweries.                                            |
| `/breweries`                      |  `POST`  | `brewery`         | Creates a new brewery resource. The `brewery.id` is determined server-side. Only allowed for the role "Brewer". Only one brewery resource can be created per brewer. | 
| `/breweries/{id}`                 | `GET`    | `brewery`         |                                                                                                                                                                      |
| `/breweries/{id}`                 | `PUT`    | `brewery`         | Updates the brewery identified by `id`. The property `brewery.id` is ignored.                                                                                        |
| `/breweries/{id}/beers`           |  `GET`   |  `beers[]`        | Gets all beers for the brewery identified by `id`.                                                                                                                   |
| `/breweries/{id}/beers`           | `POST`   |  `beer`           | Creates a new beer resource. Only allowed for the brewer that "owns" the brewery identified by `id`.                                                                 |
| `/breweries/{id}/beers/{beer_id}` |  `GET`   | `beer`            | Gets the beer identified by `beer_id`.                                                                                                                               |
| `/breweries/{id}/beers/{beer_id}` | `PUT`    | `beer`            | Updates the beer identified by `beer_id`. `beer.id` is ignored. Only allowed for the brewer that "owns" the brewery identified by `id`.                              |
| `/breweries/{id}/beers/{beer_id}` | `DELETE` | `--`              | Deletes the beer identified by `beer_id`. Only allowed for the brewer that "owns" the brewery identified by `id`.                                                    |
| `/wholesales`                     |  `GET`   |  `wholesale[]`    | Gets all wholesales for the "Client" role. For the "Seller" role this only returns the associated wholesales.                                                        |
| `/wholesales`                     | `POST`   | `wholesale`       | Creates a new wholesale resource. The `wholesale.id` is determined server-side. Only allowed for the "Seller" role.                                                  |
| `/wholesales/{id}`                | `GET`    | `wholesale`       | Gets the wholesale identified by `id`.                                                                                                                               |
| `/wholesales/{id}/requestQuote`   |  `POST`  | `order`           |                                                                                                                                                                      |  
| `/wholesales/{id}/inventory`      | `GET`    | `inventoryItem[]` |                                                                                                                                                                      |

### `user`

| Property | Type     | Description                                                                   |
|----------|----------|-------------------------------------------------------------------------------|
|  `id`    | `string` | The unique identifier of the user.                                            |
| `role`   | `string` | The role associated with the user. May be one of "Client", "Brewer", "Seller" |

### `brewery`

|  Property | Type           | Description                                      |
|-----------|----------------|--------------------------------------------------|
| `id`      | `string`       | The unique identifier of the brewery. Read-only. |
| `name`    | `string`       | The name of the brewery.                         |
| `links`   | `breweryLinks` | The links the application can follow. Read-only. |

### `breweryLinks`

|  Property | Type     | Description                                        |
|-----------|----------|----------------------------------------------------|
| `self`    | `string` | The link to the resource itself.                   |
| `beers`   | `string` | The link to obtain the list of beers of a brewery. | 

### `beer`

|  Property      | Type          | Description                                       |
|----------------|---------------|---------------------------------------------------|
| `id`           | `string`      | The unique identifier of the beer. Read-only.     |
| `name`         | `string`      | The name of the beer.                             |
| `description`  | `string`      | A description of the beer.                        |
| `price`        | `number`      | The price per unit.                               | 
|  `availableAt` | `wholesale[]` |  The wholesales the beer is available at.         |
| `links`        | `beerLinks`   |  The links the application can follow. Read-only. |

### `wholesale`

|  Property | Type             | Description                                         |
|-----------|------------------|-----------------------------------------------------|
| `id`      |  `string`        |  The unique identifier of the wholesale. Read-only. |
| `name`    | `string`         | The name of the wholesale.                          |
| `links`   | `wholesaleLinks` | The links the application can follow. Read-only.    |

### `wholesaleLinks`

|  Property      | Type     | Description                                          |
|----------------|----------|------------------------------------------------------|
| `self`         | `string` | The link to the resource itself.                     |
| `inventory`    | `string` | The link to the wholesales inventory.                |
| `requestQuote` | `string` | The link to request a quote. Only for "Client" role. |

### `order`

|  Property | Type          | Description                                    |
|-----------|---------------|------------------------------------------------|
| `id`      | `string`      | The unique identifier of the order. Read-only. |
| `items`   | `orderItem[]` | The items to order.                            |

### `orderItem`

|  Property | Type | Description |
|-----------|------|-------------|

### `inventoryItem`

|  Property | Type | Description |
|-----------|------|-------------|

## RabbitMQ messages