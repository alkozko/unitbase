namespace  Demostore

open Nancy

module App =
    
    type App() as this =
        inherit NancyModule()