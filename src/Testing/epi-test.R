# epi-test.R
#
# Tests the epi package

# Initialize library -----------

library(rsyncrosim)

ssimFolder <- "C:/Users/colin.daniel/Documents/SyncroSim/Install/2-2-40"
# ssimFolder <- "C:/gitprojects/ssimbin"

setwd("c:/temp/epi")
getwd()

# Create a new epi .ssim file from scratch (with add-ons enabled)
mySession <- session(ssimFolder)
myLibrary <- ssimLibrary("epi.ssim", session=mySession, package="epi", overwrite=T)
enableAddon(myLibrary, c(
  "epiDataBc",
  "epiDataCanada",
  "epiDataWorld",
  "epiModelRegression",
  "modelCovidseir",
  "modelKarlenPypm"))

# Create a blank scenario to hold the BC data (automatically assigned to project 1)
myScenarioData <- scenario(myLibrary, "BC Data")
myProject <- project(myScenarioData)
name(myProject) <- "Definitions"

# Generate various reference lists
# datasheetReference <- datasheet(myScenarioData)
# transformerReference <- datasheet(myScenarioData, "core_Transformer")
# stageReference <- datasheet(myScenarioData, "core_StageValue")
# variableReference <- datasheet(myScenarioData, "epi_Variable", includeKey = T)

# Create a chart of Cases
# myData = data.frame(
#   Name = "Cases",
#   ChartType = "Line Chart",
#   ErrorBarType = "No Ranges",
#   Criteria = "epi_Variable-15|epi_Variable-16",
#   CriteriaXVariable = NA)
# saveDatasheet(myScenarioData, myData, "corestime_Charts")

# epiDataBc scenario  ----------

# Set the pipeline for the data scenario
# Note that by default the Stage gets the Transformer's description 
myData = data.frame(StageNameID = "Download BC CDC Data", RunOrder = 1)
saveDatasheet(myScenarioData, myData, "core_Pipeline")

# covidseir onset delay scenario  ----------

# Create a second scenario for the onset delay data
myScenarioOnset <- scenario(myLibrary, "Onset Delay Data")

# Set the pipeline for the onset delay scenario
myData = data.frame(StageNameID = "covidseir model: Download and fit delay data", RunOrder = 1)
saveDatasheet(myScenarioOnset, myData, "core_Pipeline")

# Run scenarios for BC data and onset delay data ----------------
resultsScenarioData <- run(myScenarioData)
resultsScenarioOnset <- run(myScenarioOnset)

# Setup  the regression model scenario  ----------

# Create a regression model scenario
myScenarioReg <- scenario(myProject, "Regression Model")

# Set the pipeline for the regression scenario
myData = data.frame(StageNameID = "Run Case Regression Model", RunOrder = 1)
saveDatasheet(myScenarioReg, myData, "core_Pipeline")

# Set the other inputs
myData = data.frame(
  MaximumTimestep = "2021-04-30",
  MaximumIteration = 1,
  RegressionWindow = 7)
saveDatasheet(myScenarioReg, myData, "epiModelRegression_RunSettings")
myData = data.frame(Jurisdictions = "Canada - British Columbia")
saveDatasheet(myScenarioReg, myData, "epiModelRegression_RunJurisdictions")

# Use RStudio -------------
myData = data.frame(UseRStudio=T)
saveDatasheet(myLibrary, myData, "core_RConfig")

