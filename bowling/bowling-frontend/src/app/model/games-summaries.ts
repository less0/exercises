import { GameSummary } from "./game-summary";

export class GamesSummaries {
    games : GameSummary[] | undefined;
    links : {[name:string] : string; } = {};
}
