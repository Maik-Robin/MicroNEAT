# MicroNEAT Test Suite

This test suite provides comprehensive coverage for the MicroNEAT neural evolution library.

## Test Organization

### 1. **ActivationFunctionTests.cs** (7 tests)
Tests all activation functions:
- Sigmoid
- ReLU
- LeakyReLU
- Tanh
- NEATSigmoid
- Gaussian
- SELU

### 2. **NEATConfigTests.cs** (4 tests)
Tests configuration management:
- Default configuration values
- Custom configuration settings
- Mutation rate validation
- Weight bounds validation

### 3. **GenomeBuilderTests.cs** (5 tests)
Tests genome creation:
- Correct node count creation
- Initial connection setup
- Bias node handling
- Unique ID assignment
- Innovation number assignment

### 4. **GenomeTests.cs** (11 tests)
Tests genome operations:
- Constructor initialization
- Propagation with different inputs
- State reset functionality
- Node retrieval by ID
- Mutation operations
- Weight reinitialization
- Genome copying

### 5. **NodeGeneTests.cs** (7 tests)
Tests individual node types:
- InputNode initialization and behavior
- OutputNode initialization and behavior
- HiddenNode initialization and behavior
- BiasNode initialization and behavior
- State reset functionality

### 6. **ConnectionGeneTests.cs** (4 tests)
Tests connection genes:
- Constructor initialization
- Disabled connection behavior
- Weight reinitialization
- Recurrent connection handling

### 7. **PopulationTests.cs** (7 tests)
Tests population-level operations:
- Population creation
- Fitness evaluation
- Best genome selection
- Speciation
- Evolution mechanics
- Population size maintenance
- Multi-generation improvement

### 8. **NEATAlgorithmTests.cs** (4 tests)
Tests the main NEAT algorithm:
- Algorithm initialization
- Best genome retrieval
- Evolution execution
- Target fitness stopping condition

### 9. **FitnessFunctionTests.cs** (4 tests)
Tests fitness evaluation:
- Valid fitness calculation
- High fitness for good solutions
- Consistent results
- All input combinations tested

### 10. **WeightInitializationTests.cs** (6 tests)
Tests weight initialization:
- Min/max range setting
- Value within range
- Correct array size
- All weights in range
- Weight variation
- Zero range handling

### 11. **SpeciesTests.cs** (9 tests)
Tests species management:
- Species initialization
- Genome addition
- Representative selection
- Adjusted fitness calculation
- Total fitness summation
- Best genome selection
- Fitness and stagnation tracking
- Stagnation detection
- Bad genome removal

### 12. **GeneticOperationTests.cs** (7 tests)
Tests genetic operations:
- Crossover operation
- Offspring independence
- Genetic encoding
- Compatibility distance calculation
- Fitness evaluation
- Genome equality comparison

### 13. **MutationTests.cs** (7 tests)
Tests mutation operations:
- Weight mutation
- Add node mutation
- Add connection mutation
- Connectivity preservation
- Weight perturbation bounds
- Recurrent connection updates
- Disabled connection effects

### 14. **EdgeCaseTests.cs** (11 tests)
Tests edge cases and boundary conditions:
- Empty inputs
- Multiple outputs
- Large networks
- Extreme weights
- Very small populations
- Multiple resets
- Zero inputs
- Negative inputs
- Empty species
- Zero weights
- Consecutive propagations

### 15. **IntegrationTests.cs** (5 tests)
End-to-end integration tests:
- Simple network propagation
- XOR problem evolution
- Structural integrity maintenance
- Different activation function behavior
- Progressive improvement over generations

### 16. **TrackerTests.cs** (8 tests)
Tests utility tracker classes:
- InnovationTracker unique numbers
- InnovationTracker same connection handling
- InnovationTracker reset
- NodeTracker ID incrementation
- GenomeTracker ID incrementation
- PopulationTracker ID incrementation
- ConfigTracker storage
- ConfigTracker differentiation

### 17. **TestSuiteSummary.cs** (1 test)
Documentation and verification test

## Total Test Coverage

**Total Tests: 106+**

All tests use NUnit 3.14.0 framework and target .NET 8.

## Running Tests

To run all tests:
```bash
dotnet test MicroNEAT.Test/MicroNEAT.Test.csproj
```

To run tests with detailed output:
```bash
dotnet test MicroNEAT.Test/MicroNEAT.Test.csproj --verbosity normal
```

To list all available tests:
```bash
dotnet test MicroNEAT.Test/MicroNEAT.Test.csproj --list-tests
```

## Coverage Areas

? **Activation Functions**: All built-in activation functions tested
? **Configuration**: Default and custom configurations validated
? **Genome Operations**: Creation, mutation, crossover, propagation
? **Node Types**: All node types (Input, Output, Hidden, Bias)
? **Connections**: Weight management, enabling/disabling, recurrent handling
? **Population**: Evolution, speciation, fitness evaluation
? **Algorithm**: Complete NEAT algorithm execution
? **Trackers**: All utility tracking classes
? **Edge Cases**: Boundary conditions and unusual scenarios
? **Integration**: End-to-end evolutionary scenarios

## Notes

The warnings during test execution are expected Console.WriteLine outputs from the NEAT algorithm showing generation progress, not actual test failures.
