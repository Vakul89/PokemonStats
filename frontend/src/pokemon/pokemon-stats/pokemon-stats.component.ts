import { Component } from '@angular/core';
import { PokemonDetail } from '../../shared/models/pokemon-stats';
import { SortBy, SortDirection } from '../../shared/models/filters';
import { PokemonStatsService } from '../../shared/services/pokemon-stats.service';
import { NgClass, TitleCasePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-pokemon-stats',
  standalone: true,
  imports: [TitleCasePipe, FormsModule, NgClass],
  templateUrl: './pokemon-stats.component.html',
  styleUrl: './pokemon-stats.component.css'
})
export class PokemonStatsComponent {
  pokemons: PokemonDetail[] = [];
  sortBy: SortBy = 'wins';
  sortDirection: SortDirection = 'desc';

  pageSize = 8;
  currentPage = 1;

  constructor(private pokemonStatsService: PokemonStatsService) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.pokemonStatsService
      .getStatistics(this.sortBy, this.sortDirection)
      .subscribe(p => this.pokemons = p);
  }

  onSortByChange(value: SortBy): void {
    this.sortBy = value;
    this.loadData();
  }

  onDirectionChange(value: SortDirection): void {
    this.sortDirection = value;
    this.loadData();
  }

  onPageSizeChange(value: number): void {
    this.pageSize = value;
    this.currentPage = 1;
    this.loadData();
  }

  get pagedPokemons(): PokemonDetail[] {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.pokemons.slice(start, start + this.pageSize);
  }

  nextPage(): void {
    if (this.currentPage * this.pageSize < this.pokemons.length) {
      this.currentPage++;
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  getWinRate(p: PokemonDetail): number {
    const total = p.wins + p.losses + p.ties;
    return total === 0 ? 0 : Math.round((p.wins / total) * 100);
  }

  getBarColor(p: PokemonDetail): string {
    const rate = this.getWinRate(p);

    if (rate >= 80) return 'bg-success';
    if (rate >= 50) return 'bg-warning';
    return 'bg-danger';
  }
}
