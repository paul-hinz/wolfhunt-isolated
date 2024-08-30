﻿using System;
using System.Diagnostics;
using System.IO;
using Mars.Components.Starter;
using Mars.Interfaces.Model;
using WolfHuntIsolated.Model;
using ServiceStack;

namespace WolfHuntIsolated; 


internal static class Program {
    public static void Main(string[] args) {
        // This scenario consists of:
        // 1. a model (represented by the model description)
        // 2. a simulation configuration (see config.json)

        // Create a new model description that holds all parts of the model (agents, entities, layers)
        var description = new ModelDescription();
        description.AddLayer<LandscapeLayer>();

        description.AddAgent<Prey, LandscapeLayer>();
        description.AddAgent<Wolf, LandscapeLayer>();

        // Scenario definition: Use config.json that holds the specification of the scenario
        var file = File.ReadAllText("config.json");
        var config = SimulationConfig.Deserialize(file);

        // Reads the TickLength in seconds and stores in a static helper class for agents to see during runtime
        Debug.Assert(config.Globals.DeltaTTimeSpan != null, "config.Globals.DeltaTTimeSpan != null");
        var timeSpan = config.Globals.DeltaTTimeSpan.Value.TotalSeconds;
        GlobalValueHelper.TickSeconds = timeSpan;
        Console.WriteLine("Seconds Per Tick: " + timeSpan);

        // Create simulation task
        var simStarter = SimulationStarter.Start(description, config);

        // Run simulation
        var results = simStarter.Run();

        // Feedback to user that simulation run was successful
        Console.WriteLine($"Simulation execution finished after {results.Iterations} steps");
       
    }
}
