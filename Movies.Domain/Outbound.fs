module Movies.Domain.Outbound

open Movies.Domain.Models

type IMoviesProvider =
  abstract member GetPopulars : Movie list
  abstract member GetUpcoming : Movie list
  abstract member GetById : id: string -> Movie option