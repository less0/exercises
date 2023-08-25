import { Frame } from "./frame"

export class GameDetails {
    public players : string[] | undefined
    public currentPlayer : string | undefined;
    public startedAt : Date | undefined
    public isInProgress : boolean | undefined
    public frames : Frame[][] | undefined
    links : {[name:string]: string;} | undefined;
}
