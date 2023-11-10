# Beers of Belgium Communication

## Introduction

This file is an addendum to the architecture documentation and serves the purpose to define the communication interfaces
between the components of the application. The basic means of communication are described in the architecture document,
this documents describes the exchanges messages in a greater depth.

## REST-API

| URI                               | Method   | Data              | Remarks                                                                                                                                                                      |
|-----------------------------------|----------|-------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `/users`                          | `GET`    | `user[]`          | Only returns `user`-objects valid for the authenticated user.                                                                                                                |
| `/users`                          | `POST`   | `user`            | Creates a new user resource. The `user.id` is determined server-side. If the role is invalid or the method is called multiply by the same user, an error status is returned. | 
| `/users/{id}/orders`              | `GET`    | `order[]`         | Gets all orders for the current user. For "Seller" and "Brewer" role, this endpoint returns the HTTP status 403.                                                             |
| `/breweries`                      | `GET`    | `brewery[]`       | Gets all breweries for the "Client" and "Seller" roles. For the Role "Brewer" this only returns the associated breweries.                                                    |
| `/breweries`                      | `POST`   | `brewery`         | Creates a new brewery resource. The `brewery.id` is determined server-side. Only allowed for the role "Brewer". Only one brewery resource can be created per brewer.         | 
| `/breweries/{id}`                 | `GET`    | `brewery`         |                                                                                                                                                                              |
| `/breweries/{id}`                 | `PUT`    | `brewery`         | Updates the brewery identified by `id`. The property `brewery.id` is ignored.                                                                                                |
| `/breweries/{id}/beers`           | `GET`    | `beers[]`         | Gets all beers for the brewery identified by `id`.                                                                                                                           |
| `/breweries/{id}/beers`           | `POST`   | `beer`            | Creates a new beer resource. Only allowed for the brewer that "owns" the brewery identified by `id`.                                                                         |
| `/breweries/{id}/beers/{beer_id}` | `GET`    | `beer`            | Gets the beer identified by `beer_id`.                                                                                                                                       |
| `/breweries/{id}/beers/{beer_id}` | `PUT`    | `beer`            | Updates the beer identified by `beer_id`. `beer.id` is ignored. Only allowed for the brewer that "owns" the brewery identified by `id`.                                      |
| `/breweries/{id}/beers/{beer_id}` | `DELETE` | `--`              | Deletes the beer identified by `beer_id`. Only allowed for the brewer that "owns" the brewery identified by `id`.                                                            |
| `/wholesales`                     | `GET`    | `wholesale[]`     | Gets all wholesales for the "Client" role. For the "Seller" role this only returns the associated wholesale.                                                                 |
| `/wholesales`                     | `POST`   | `wholesale`       | Creates a new wholesale resource. The `wholesale.id` is determined server-side. Only allowed for the "Seller" role.                                                          |
| `/wholesales/{id}`                | `GET`    | `wholesale`       | Gets the wholesale identified by `id`.                                                                                                                                       |
| `/wholesales/{id}/requestQuote`   | `POST`   | `order`           | Requests a new quote. Only allowed for "Client" role.                                                                                                                        |  
| `/wholesales/{id}/inventory`      | `GET`    | `inventoryItem[]` | Gets all inventory items for the wholesale identified by `id`. Only allowed for "Client" and "Wholesale" role.                                                               |
| `/wholesales/{id}/inventory`      | `POST`   | `inventoryItem`   | Adds a new inventory item for the wholesale. If there is an inventory item with the same `beer_id`, the amounts are added up. Only allowed for the owner of the wholesale.   |

### `user`

| Property | Type        | Description                                                                   |
|----------|-------------|-------------------------------------------------------------------------------|
| `id`     | `string`    | The unique identifier of the user.                                            |
| `role`   | `string`    | The role associated with the user. May be one of "Client", "Brewer", "Seller" |
| `links`  | `userLinks` | The links the application can follow.                                         |

### `userLinks`

| Property | Type     | Description                                                                      |
|----------|----------|----------------------------------------------------------------------------------|
| `self`   | `string` | The link to the resource itself.                                                 |
| `orders` | `string` | The link to the orders associated with the current user. Only for "Client" role. | 

### `brewery`

| Property | Type           | Description                                      |
|----------|----------------|--------------------------------------------------|
| `id`     | `string`       | The unique identifier of the brewery. Read-only. |
| `name`   | `string`       | The name of the brewery.                         |
| `links`  | `breweryLinks` | The links the application can follow. Read-only. |

### `breweryLinks`

| Property | Type     | Description                                        |
|----------|----------|----------------------------------------------------|
| `self`   | `string` | The link to the resource itself.                   |
| `beers`  | `string` | The link to obtain the list of beers of a brewery. | 

### `beer`

| Property      | Type          | Description                                         |
|---------------|---------------|-----------------------------------------------------|
| `id`          | `string`      | The unique identifier of the beer. Read-only.       |
| `name`        | `string`      | The name of the beer.                               |
| `description` | `string`      | A description of the beer.                          |
| `price`       | `number`      | The price per unit.                                 | 
| `availableAt` | `wholesale[]` | The wholesales the beer is available at. Read only. |
| `links`       | `beerLinks`   | The links the application can follow. Read-only.    |

### `wholesale`

| Property | Type             | Description                                        |
|----------|------------------|----------------------------------------------------|
| `id`     | `string`         | The unique identifier of the wholesale. Read-only. |
| `name`   | `string`         | The name of the wholesale.                         |
| `links`  | `wholesaleLinks` | The links the application can follow. Read-only.   |

### `wholesaleLinks`

| Property       | Type     | Description                                          |
|----------------|----------|------------------------------------------------------|
| `self`         | `string` | The link to the resource itself.                     |
| `inventory`    | `string` | The link to the wholesales inventory.                |
| `requestQuote` | `string` | The link to request a quote. Only for "Client" role. |

### `order`

| Property    | Type          | Description                                          |
|-------------|---------------|------------------------------------------------------|
| `id`        | `string`      | The unique identifier of the order. Read-only.       |
| `items`     | `orderItem[]` | The items to order.                                  |
| `orderedAt` | `string`      | UTC-timestamp of the order in ISO format. Read-only. |
| `address`   | `string`      | The users shipping address.                          |
| `discount`  | `number`      | The percentage the order is discounted. Read-only.   |
| `total`     | `number`      | The total sum of the order. Read-only.               |

### `orderItem`

| Property       | Type     | Description                                         |
|----------------|----------|-----------------------------------------------------|
| `id`           | `string` | The unique identifier of the order item. Read only. |
| `beerId`       | `string` | The unique identifier of the beer to be ordered.    |
| `name`         | `string` | The name of the item. Read-only.                    |
| `amount`       | `number` | Number of bottles to order. Must be an integer.     |
| `pricePerUnit` | `number` | The price per unit. Read-only.                      |

### `inventoryItem`

| Property  | Type     | Description                                             |
|-----------|----------|---------------------------------------------------------|
| `id`      | `string` | The unique identifier of the inventory item. Read-only. |
| `beer_id` | `string` | The unique identifier of the beer.                      |
| `amount`  | `number` | The number of bottles to add to the inventory.          |

## `bob-usermanagement`-API

| URI          | Method | Data   |                                                                                |
|--------------|--------|--------|--------------------------------------------------------------------------------|
| `/user       | `POST` | `user` | Creates the specified user. Role and id have to be provided.                   |
| `/user/{id}` | `GET`  | `user` | Gets the user with the specified id. 404 if the user has not been created yet. |

## RabbitMQ messages