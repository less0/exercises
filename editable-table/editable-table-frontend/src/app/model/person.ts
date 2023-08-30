import { Department } from "./department"

export class Person {
    id! : string
    firstName! : string
    lastName! : string
    dateOfBirth! : Date
    department! : Department

    isInEditMode : boolean = false;

    links! : { [name: string]: string; }
}
