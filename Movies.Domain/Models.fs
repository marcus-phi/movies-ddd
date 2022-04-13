module Movies.Domain.Models

open System

type Rate = Rate of float

type Person = { Name: string; FirstName: string }

type Movie =
  { Id: string
    Title: string
    Synopsys: string
    Rate: Rate
    ReleaseDate: DateOnly
    Director: Person
    Actors: Person list }
