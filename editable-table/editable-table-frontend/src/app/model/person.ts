import { Department } from "./department"

export class Person {
    firstName! : string
    lastName! : string
    dateOfBirth! : Date
    department! : Department

    links! : { [name: string]: string; }
}
