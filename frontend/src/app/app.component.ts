import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PokemonStatsComponent } from '../pokemon/pokemon-stats/pokemon-stats.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, PokemonStatsComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'frontend';
}
