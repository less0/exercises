export class GameSummary {
    id : string | undefined;
    startedAt : Date | undefined;
    numberOfPlayers : number | undefined;
    isInProgress : boolean | undefined;
    links : {[name:string]: string;} | undefined;
}
