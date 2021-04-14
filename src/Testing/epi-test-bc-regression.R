# epi-test-bc-regression.R
#
# Tests the epi package (running regression model on BC CDC data)

library(rsyncrosim)

# Set script variables  -----------

# Set how to run the script
rebuild = F  # Set to TRUE to rebuild library from scratch; otherwise re-runs regression only
debug = F  # Set to TRUE to launch regression in RStudio
useUI = F  # Set to TRUE to use Windows UI to run regression

# Set working directory to where SyncroSim library will be created
setwd("c:/temp/epi")

# Set folder where SyncroSim is installed:
ssimFolder <- "C:/Users/colin.daniel/Documents/SyncroSim/Install/2-2-41"

# Rebuild library and download data -----------

mySession <- session(ssimFolder)
if (rebuild) {
  # Create a new epi.ssim library file from scratch based on the epi base package
  myLibrary <- ssimLibrary("epi.ssim", session=mySession, package="epi", overwrite=T)
  
  # Enable the SyncroSim add-on packages (to the epi base package)
  enableAddon(myLibrary, c("epiDataBc", "epiModelRegression"))
  
  # Create a blank scenario to hold the BC data
  myScenarioData <- scenario(myLibrary, "BC CDC Data")
  
  # # Get a handle on the scenario's project
  # myProject <- project(myScenarioData)
  
  # Generate various reference lists - for developer reference in RStudio only
  datasheetReference <- datasheet(myScenarioData)
  stageReference <- datasheet(myScenarioData, "core_StageName")
  
  # Set the pipeline for the data scenario
  # Note that by default the Stage gets the Transformer's description 
  myData = data.frame(StageNameID = "Download BC CDC Data", RunOrder = 1)
  saveDatasheet(myScenarioData, myData, "core_Pipeline")

  # Create a chart of Cases - Note Variable IDs can change
  # datasheet(myScenarioData, "epi_Variable", includeKey = T)
  # chartReference = datasheet(myScenarioData, "corestime_Charts")
  myData = data.frame(
    Name = "Cases",
    ChartType = "Line Chart",
    TimestepsLine = "2021-03-01 - NULL",
    ErrorBarType = "Min/Max",
    Criteria = "epi_Variable-14|epi_Variable-15",
    CriteriaXVariable = NA)
  saveDatasheet(myScenarioData, myData, "corestime_Charts")

  # Run the data download scenario
  resultsScenarioData <- run(myScenarioData)

  # Create new scenario for the regression model
  myScenarioReg <- scenario(myLibrary, "Regression Model")
  
  # Set the pipeline for the regression scenario
  myData = data.frame(StageNameID = "Run Case Regression Model", RunOrder = 1)
  saveDatasheet(myScenarioReg, myData, "core_Pipeline")
  
  # Create a dependency of the regression scenario on the BC CDC data
  dependency(myScenarioReg, dependency=myScenarioData)
} else
{
  # Load the existing library and regression scenario
  myLibrary <- ssimLibrary("epi.ssim", session=mySession, package="epi", overwrite=F)
  myScenarioReg <- scenario(myLibrary, "Regression Model")
}

# Generate regression model scenario  ----------

# Set the inputs for the regression
myData = data.frame(
  MaximumTimestep = "2021-06-30",
  MaximumIteration = 6,
  RegressionWindow = 7)
saveDatasheet(myScenarioReg, myData, "epiModelRegression_RunSettings")
myData = data.frame(Jurisdictions = "Canada - British Columbia")
saveDatasheet(myScenarioReg, myData, "epiModelRegression_RunJurisdictions")

if (debug) {
  # Invoke RStudio when running regression
  myData = data.frame(UseRStudio=T)
  saveDatasheet(myLibrary, myData, "core_RConfig")
}

if (!useUI) {
  # Run the regression directly if not using Windows UI
  resultsScenarioReg <- run(myScenarioReg, jobs=6)
}
