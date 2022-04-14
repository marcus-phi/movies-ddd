open Microsoft.FSharp.Core
open Movies.Domain.Outbound
open Movies.Domain.Services
open Movies.Infra.Services
open TMDbLib.Client

let printUsage () =
  printfn "Usage: -apiKey <apiKey> <command>"
  printfn "Commands:"
  printfn "    -popular - Get popular movies"
  printfn "    -upcoming - Get upcoming movies"
  printfn "    -get <id> - Get movie by id"

let getMoviesService apiKey =
  let client = new TMDbClient(apiKey)

  let moviesProvider =
    TheMoviesDBService(client) :> IMoviesProvider

  MoviesServiceImpl(moviesProvider) :> IMoviesService

[<EntryPoint>]
let main args =

  let arguments = List.ofArray args

  match arguments with
  | "-apiKey" :: apiKey :: "-popular" :: _ ->
    (getMoviesService apiKey).GetPopulars()
    |> Seq.iter (fun m -> printfn $"%A{m}")
  | "-apiKey" :: apiKey :: "-upcoming" :: _ ->
    (getMoviesService apiKey).GetUpcoming()
    |> Seq.iter (fun m -> printfn $"%A{m}")
  | "-apiKey" :: apiKey :: "-get" :: id :: _ ->
    (getMoviesService apiKey).GetById id
    |> printfn "%A"
  | _ -> printUsage ()

  0
