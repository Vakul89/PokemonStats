import { Injectable } from '@angular/core';
import { PokemonDetail } from '../models/pokemon-stats';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs/internal/Observable';
import { SortBy, SortDirection } from '../models/filters';
import { environment } from '../../environment/environment.development';

@Injectable({
  providedIn: 'root'
})

export class PokemonStatsService {

  private readonly baseUrl = environment.apiUrl + 'pokemon/tournament/statistics';

  constructor(private http: HttpClient) { }

  getStatistics(sortBy: SortBy, sortDirection: SortDirection = 'asc'):
    Observable<PokemonDetail[]> {
    let params = new HttpParams()
      .set('sortBy', sortBy)
      .set('sortDirection', sortDirection);

    return this.http.get<PokemonDetail[]>(this.baseUrl, { params });
  }
}