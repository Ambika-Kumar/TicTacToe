import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
export type Player = 0 | 1;
export type GameMode = 0 | 1;
export interface Move { moveNumber:number; player:Player; row:number; column:number; }
export interface Game { id:string; boardView:(Player|null)[][]; currentPlayer:Player; mode:GameMode; status:number; winner:Player|null; winningCells:string[]; moveHistory:Move[]; scoreRecorded:boolean; }
export interface Scoreboard { xWins:number; oWins:number; draws:number; }
@Injectable({providedIn:'root'}) export class GameService {
 private http=inject(HttpClient); private base='http://localhost:5241/api';
 createGame(mode:GameMode):Observable<Game>{return this.http.post<Game>(`${this.base}/games`,{mode});}
 move(id:string,player:Player,row:number,column:number):Observable<Game>{return this.http.post<Game>(`${this.base}/games/${id}/moves`,{player,row,column});}
 undo(id:string):Observable<Game>{return this.http.post<Game>(`${this.base}/games/${id}/undo`,{});}
 reset(id:string):Observable<Game>{return this.http.post<Game>(`${this.base}/games/${id}/reset`,{});}
 scoreboard():Observable<Scoreboard>{return this.http.get<Scoreboard>(`${this.base}/scoreboard`);}
 resetScoreboard():Observable<Scoreboard>{return this.http.post<Scoreboard>(`${this.base}/scoreboard/reset`,{});}
}
