# epi-test.R
#
# Tests the epi package

# Initialize library -----------

library(rsyncrosim)

# Set folder where SyncroSim is installed:
ssimFolder <- "C:/Users/colin.daniel/Documents/SyncroSim/Install/2-2-41"
# ssimFolder <- "C:/gitprojects/ssimbin"

# Set working directory to where SyncroSim library will be created
setwd("c:/temp/epi")
getwd()

# Create a new epi.ssim library file from scratch based on the epi base package
mySession <- session(ssimFolder)
myLibrary <- ssimLibrary("epi.ssim", session=mySession, package="epi", overwrite=T)

# Enable the SyncroSim add-on packages (to the epi base package)
enableAddon(myLibrary, c("epiDataBc", "epiModelRegression"))

# enableAddon(myLibrary, c(
#   "epiDataBc",
#   "epiDataCanada",
#   "epiDataWorld",
#   "epiModelRegression",
#   "modelCovidseir",
#   "modelKarlenPypm"))


# Create a chart of Cases - used only on the Windows UI
# myData = data.frame(
#   Name = "Cases",
#   ChartType = "Line Chart",
#   ErrorBarType = "No Ranges",
#   Criteria = "epi_Variable-15|epi_Variable-16",
#   CriteriaXVariable = NA)
# saveDatasheet(myScenarioData, myData, "corestime_Charts")

# Create a blank scenario to hold the BC data (automatically assigned to project 1)
myScenarioData <- scenario(myLibrary, "BC CDC Data")
# myProject <- project(myScenarioData)
# name(myProject) <- "Definitions"

# Generate various reference lists - for reference in RStudio only
datasheetReference <- datasheet(myScenarioData)
stageReference <- datasheet(myScenarioData, "core_StageName")

# epiDataBc scenario  ----------

# Set the pipeline for the data scenario
# Note that by default the Stage gets the Transformer's description 
myData = data.frame(StageNameID = "Download BC CDC Data", RunOrder = 1)
saveDatasheet(myScenarioData, myData, "core_Pipeline")

# Use RStudio -------------
myData = data.frame(UseRStudio=T)
saveDatasheet(myLibrary, myData, "core_RConfig")

# Run scenarios for BC data and onset delay data ----------------
resultsScenarioData <- run(myScenarioData)



# covidseir onset delay scenario  ----------

# Create a second scenario for the onset delay data
myScenarioOnset <- scenario(myLibrary, "Onset Delay Data")

# Set the pipeline for the onset delay scenario
myData = data.frame(StageNameID = "covidseir model: Download and fit delay data", RunOrder = 1)
saveDatasheet(myScenarioOnset, myData, "core_Pipeline")

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

resultsScenarioData <- run(myScenarioReg)


# Use RStudio -------------
myData = data.frame(UseRStudio=T)
saveDatasheet(myLibrary, myData, "core_RConfig")

