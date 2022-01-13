namespace Demo.Orders.Domain.Shared

/// A common set of functions that are implemented for each 
/// aggregate. They abstract the Decider pattern from our 
/// domain-specific code. 
[<NoEquality>]
[<NoComparison>]
type Decider<'command,'event, 'state, 'error> =
    {
        decide: 'command -> 'state -> Result<'event list, 'error>
        evolve: 'state -> 'event -> 'state
        initialState: 'state
        isTerminal: 'state -> bool
    }

module Decider =
    let currentState decider state events =
        (state, events) ||> List.fold decider.evolve