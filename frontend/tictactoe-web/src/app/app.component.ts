import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Game, GameMode, GameService, Player } from './game.service';
@Component({selector:'app-root',standalone:true,imports:[CommonModule,FormsModule],templateUrl:'./app.component.html',styleUrl:'./app.component.css'})
export class AppComponent {
 private api=inject(GameService); game:Game|null=null; mode:GameMode=0; loading=false; error=''; scoreboard={xWins:0,oWins:0,draws:0}; readonly cells=Array.from({length:9},(_,i)=>i);
 constructor(){this.newGame();this.refreshScoreboard();}
 newGame(){this.loading=true;this.api.createGame(this.mode).subscribe({next:g=>{this.game=g;this.loading=false},error:e=>this.fail(e)});}
 //play(i:number){if(!this.game||this.loading||this.game.status!==0||this.value(i)!==null)return;this.loading=true;this.api.move(this.game.id,this.game.currentPlayer,Math.floor(i/3),i%3).subscribe({next:g=>{this.game=g;this.loading=false;this.refreshScoreboard()},error:e=>this.fail(e)});}
 play(i:number){
  if(!this.game || this.loading || this.game.status !== 0)
    return;

  if(this.value(i) !== null){
    this.error = 'Invalid move: Cell is already occupied.';
    return;
  }

  this.error = '';
  this.loading = true;

  this.api.move(
    this.game.id,
    this.game.currentPlayer,
    Math.floor(i / 3),
    i % 3
  ).subscribe({
    next: g => {
      this.game = g;
      this.loading = false;
      this.refreshScoreboard();
    },
    error: e => this.fail(e)
  });
}
 undo(){if(!this.game)return;this.loading=true;this.api.undo(this.game.id).subscribe({next:g=>{this.game=g;this.loading=false},error:e=>this.fail(e)});}
 resetGame(){if(!this.game)return;this.loading=true;this.api.reset(this.game.id).subscribe({next:g=>{this.game=g;this.loading=false},error:e=>this.fail(e)});}
 resetScoreboard(){this.api.resetScoreboard().subscribe({next:s=>this.scoreboard=s,error:e=>this.fail(e)});}
 refreshScoreboard(){this.api.scoreboard().subscribe({next:s=>this.scoreboard=s,error:e=>this.fail(e)});}
 value(i:number):Player|null|undefined{return this.game?.boardView[Math.floor(i/3)][i%3];}
 isWinning(i:number){if(!this.game)return false;return this.game.winningCells.includes(`Row ${Math.floor(i/3)+1}, Column ${i%3+1}`);}
 label(p:Player|null|undefined){return p===0?'X':p===1?'O':'';}
 statusText(){if(!this.game)return '';if(this.game.status===1)return `${this.label(this.game.winner)} wins!`;if(this.game.status===2)return 'Draw game';return this.game.mode===1&&this.game.currentPlayer===1?'Computer is thinking…':`${this.label(this.game.currentPlayer)}'s turn`;}
 private fail(e:any){this.loading=false;this.error=e?.error?.message||'Unable to complete request.';}
}
