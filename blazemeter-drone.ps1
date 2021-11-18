param (
    [string]$apikey="b9107809a101b8b788e9964f",
    [string]$apisecret="24da75ad1302cfb986fe84846288e3c9aecb61742a269f0927db60b286aedbf4f9c28df6",
    [string]$testurl='https://a.blazemeter.com/app/#/accounts/140171/workspaces/438644/projects/538695/tests/9092315/edit' ,
	[string]$showtaillog ='true',
	[string]$createtest ='false',
	[string]$testname ,	
	[string]$projectid,
    [string]$inputallfiles,
	[string]$inputstartfile,
	[string]$totalusers,
	[string]$duration,
	[string]$rampup,
	[string]$Uploadfilechk
)	
	
	
		
Function StartTest([string]$StartTestid,[string] $multitests)
{	
	$StartTestResponse="";
	try{			
			if($multitests -eq 'true')
			{
	             $StartTestURL = 'https://a.blazemeter.com/api/v4/multi-tests/' +$StartTestid +'/start?delayedStart=true';
			}
			else
			{
		     	$StartTestURL = 'https://a.blazemeter.com/api/v4/tests/' +$StartTestid +'/start';
			}
			
			try {				 
				$StartTestResponse = Invoke-RestMethod $StartTestURL -Method POST -ContentType 'application/json' -Headers $hdrs;

				} 
				catch
				{				
					<# $formatstring = "{0} : {1}`n{2}`n" +
									"    + CategoryInfo          : {3}`n" +
									"    + FullyQualifiedErrorId : {4}`n"
					$fields = $_.InvocationInfo.MyCommand.Name,
							  $_.ErrorDetails.Message,
							  $_.InvocationInfo.PositionMessage,
							  $_.CategoryInfo.ToString(),
							  $_.FullyQualifiedErrorId

					$formatstring -f $fields
					#>
					
					
					$statuscode = $_.Exception.Response.StatusCode.value__ ;				    
					if($statuscode -eq '401')
					{
					 Write-Host "Test Result: Unauthorized. Please check API Key and API Secret."; 
					 Write-Host "##vso[task.complete result=Failed;]DONE";		
					 exit 1;
					}
					else
					{		
					 Write-Host "Unable to start the test. For more details check below error details."; 
					 Write-Host "##vso[task.complete result=Failed;]DONE";		
					 exit 1;						
					}
					Write-Host "Error Code:" $_.Exception.Response.StatusCode.value__ ;
					Write-Host "Error Details:" $_.ErrorDetails.Message; exit;
					
				}
				
			if($StartTestResponse -ne "")
			{

			  # Write-Host "Got response"
				$data2 = $StartTestResponse | ConvertTo-Json;
				$jsonObj2 = $data2 | ConvertFrom-Json;
				$resultObj2 = $jsonObj2.result | ConvertTo-Json;
				$resultObj2 = $resultObj2 | ConvertFrom-Json;

				$sessionId=$resultObj2.sessionsId;
				$masterId=$resultObj2.id;
				Write-Host "We are working on starting your test";

				$status='';
				$completed="ENDED";
				Write-Host "Note: Starting your test takes around 2-4 minutes. Your report will appear once we have gathered the data."

				#$ReportSummary = "https://a.blazemeter.com/app/#/accounts/"+$AccountID+"/workspaces/"+$WorkSpaceID+"/projects/"+$ProjectID+"/masters/"+$masterId+"/summary"
				$ReportSummary = "https://a.blazemeter.com/app/#/masters/"+$masterId
				Write-Host "Report URL: " $ReportSummary  -ForegroundColor green;	


				$OldStatus="";
				$output = "BlazeLogFile.txt"
				if($sessionId -ne "")
				{
		
						$TailLine=0;
				
						While ($status -ne $completed)
								
								{	
			
				 
								$statusURL ='https://a.blazemeter.com/api/v4/masters/' +$masterId +'/status';

								$LogFileURL ='https://a.blazemeter.com/api/v4/sessions/' +$sessionId[0] +'/reports/logs/data';
				
								$StatusResponse = Invoke-RestMethod $statusURL  -Method GET -ContentType 'application/json' -Headers $hdrs;

								$StatusJSONResponse =$StatusResponse | ConvertTo-Json;

								$StatusJSONFromResponse = $StatusJSONResponse | ConvertFrom-Json;

								$resultObj1 = $StatusJSONFromResponse.result | ConvertTo-Json;

								$resultObj4 = $resultObj1 | ConvertFrom-Json;

			
							   $OldStatus = $status ;
							   $status=$resultObj4.status;
									if(	$OldStatus -ne $status)
									{
										Write-Host "Test Status: " $status -ForegroundColor green;		
									}
									if($showtaillog -eq "true")	
									{			
										 if($status -eq "DATA_RECEIVED" -Or  $status -eq "TERMINATING" -Or  $status -eq "TAURUS BZT DONE" -Or  $status -eq "TAURUS IMAGE DONE" -Or  $status -eq "ENDED" )
										{
			
						
					
												$LogResponse = Invoke-RestMethod  $LogFileURL  -Method GET -ContentType 'application/json' -Headers $hdrs;

												$LogJSONResponse =$LogResponse | ConvertTo-Json;

												$LogJSONFromResponse = $LogJSONResponse | ConvertFrom-Json;

												$resultObj5 = $LogJSONFromResponse.result | ConvertTo-Json;

												$resultObj6 = $resultObj5 | ConvertFrom-Json;
												$DataLogFileURL="";
			
												For ($i=0; $i -lt $resultObj6.Length; $i++) {
													if($fileName=$resultObj6[$i].filename -eq "bzt.log")
													{
													   $DataLogFileURL =$resultObj6[$i].dataUrl	;	  				  
													   # $RedFileWebResponse= Invoke-RestMethod $DataLogFileURL
													   # Write-Host $RedFileWebResponse;
					 
														Start-Sleep -Seconds 10;    
														Invoke-WebRequest -Uri $DataLogFileURL -OutFile $output				  
														$Line = Get-Content $output | Measure-Object -Line
										
						
														$AllLines = $Line.Lines;
						
														if($AllLines -gt $TailLine){	
														$WaitMsgShown = "true"				
														$NewTailLine=$AllLines - $TailLine	;
														$TailLine = $AllLines	
										
														Get-Content  -Path $output  -Tail $NewTailLine
														Write-Host "Please wait we are gathering your data..." -ForegroundColor yellow;	
														}
													}
												}

										}
				
							        }
			  


						        }


						try
						{
							   #Write-Host "Waiting for Status  -------"
							   Start-Sleep -Seconds 10; 
								#$ThresholdURL = 'https://a.blazemeter.com/api/v4/masters/' +$masterId +'/reports/thresholds';
						
								$ThresholdURL = 'https://a.blazemeter.com/api/v4/masters/' +$masterId ;
								
								$ThresholdResponse = Invoke-RestMethod $ThresholdURL -Method GET -ContentType 'application/json' -Headers $hdrs;
								#Write-Host "TEST STATUS URL ------------" $ThresholdURL ;	
								Write-Host "Report URL: " $ReportSummary  -ForegroundColor green;
								$StatusJSONResponse1 =$ThresholdResponse | ConvertTo-Json;

								$StatusJSONFromResponse1 = $StatusJSONResponse1 | ConvertFrom-Json;

								$resultObj2 = $StatusJSONFromResponse1.result | ConvertTo-Json;

								$resultObj5 = $resultObj2 | ConvertFrom-Json;

								#Write-Host "Satus -----------" 	$resultObj5
								$APIStatus =  $resultObj5.passed;
								#Write-Host "API STATUS " $APIStatus
								if($APIStatus -eq $true -Or ($createtest -eq "true"))
								{
									#[System.Windows.Forms.MessageBox]::Show("Found")
									Write-Host "Test Execution Done." -ForegroundColor green;
								}
								ElseIf($APIStatus -eq $false)
								{
								 Write-Host "Test failed because one or more failure conditions are met.";
								 Write-Host "##vso[task.complete result=Failed;]DONE";		
								 exit 1;										
								   #[System.Windows.Forms.MessageBox]::Show("NOT Found")
								}
								else{
								      Write-Host "##vso[task.complete result=Succeeded;]DONE";
								      exit 1;	
								 }

							
						}
						catch{
						           
								$statuscode = $_.Exception.Response.StatusCode.value__ ;
								if($statuscode -eq '401')
								{
								 #Write-Error "Test Result: Unauthorized. Please check API Key and API Secret.";
								 Write-Host "Test Result: Unauthorized. Please check API Key and API Secret.";
								 Write-Host "##vso[task.complete result=Failed;]DONE";		
								 exit 1;	
													
								}
								else{					 
								 #Write-Error "Error in updating test. For more details check below error details."
								 Write-Host "Error in updating test. For more details check below error details.";
								 Write-Host "##vso[task.complete result=Failed;]DONE";		
								 exit 1;	
								}					
									
								Write-Host "Error Code:" $_.Exception.Response.StatusCode.value__ 
								Write-Host "Error Details:" $_.ErrorDetails.Message; exit;
						}

				}
				else
				{
					 Write-Host "Un Authorization";
				}
			}
			else
			{			        
					Write-Host "Unable to start the test. For more details check below error details.";  
					Write-Host "##vso[task.complete result=Failed;]DONE";		
					exit 1;	

			}
					
					
		}
		Catch
		{
		Write-Host "Error Code:" $_.Exception.Response.StatusCode.value__ 
     	Write-Host "Error Details:" $_.ErrorDetails.Message	; exit;
		}	

					try
					{
						Set-Content -Path $output -Value $_
					}	
					catch
					{}		

}	
	

Function CheckIfFileUploadedOrNot([string]$Testid)
{
	#Write-Host "Checking test files uploaded or not.";
	$ValidateFile="";$resultObj4="";
		 try
			{
				$ValidateFileURL= "https://a.blazemeter.com/api/v4/tests/"+$Testid+"/files"
				$ValidateFile = Invoke-RestMethod $ValidateFileURL -Method GET -ContentType 'application/json' -Headers $hdrs;
			}
			catch
			{
				
					$statuscode = $_.Exception.Response.StatusCode.value__ ;
					if($statuscode -eq '401')
					{
					  Write-Host "Test Result: Unauthorized. Please check API Key and API Secret."; 
					  Write-Host "##vso[task.complete result=Failed;]DONE";		
					  exit 1;	
					 					
					}
					else{	
						 Write-Host "Error in updating test. For more details check below error details."; 
					     Write-Host "##vso[task.complete result=Failed;]DONE";		
					     exit 1;						
					}					
									
					Write-Host "Error Code:" $_.Exception.Response.StatusCode.value__ 
					Write-Host "Error Details:" $_.ErrorDetails.Message; exit;
			}	
			
					$data4 = $ValidateFile | ConvertTo-Json;
					$jsonObj4 = $data4 | ConvertFrom-Json;
					$resultObj4 = $jsonObj4.result | ConvertTo-Json;
					$resultObj4 = $resultObj4 | ConvertFrom-Json;
					
					if(-not [string]::IsNullOrEmpty($resultObj4 ))
					{
					   Write-Host "Test file(s) uploaded successfully.";
					   return "true";
					}
					else
					{ 
						Write-Host "Test file(s) not found.";
						return "false";
					}
					
}
	
	
Function UpdateTest([string]$testid)
{
	try{
                # [System.Windows.Forms.MessageBox]::Show($testid);
   
				#Write-Host "Uploding file for testid: " $testid;
				#Write-Host "Auth key in Function " $AuthorizationKey;
				
				$UpdateTestURL = 'https://a.blazemeter.com/api/v4/tests/'+$testid+'/files';
												
				try
				{ 
				    # Create a web client object
					$wc = New-Object System.Net.WebClient
					$wc.Headers.Add("Authorization", $AuthorizationKey)							
					# We are redirecting to null here because web client has a little output bug where it sometimes puts some garbage
					# characters on the screen.  This has no impact on the fidelity of the copy.
					if($createtest -eq "true" )
					{
					   #Write-Host "Uploding test start file " $inputstartfile
					   $wc.UploadFile($UpdateTestURL,$inputstartfile) > $null;		
					   
					}
					if($Uploadfilechk -eq "true" )
					{
						$DependantTestfiles = Get-ChildItem -Path $inputallfiles -Force -Recurse  -file 
						for ($i=0; $i -lt $DependantTestfiles.Count; $i++) {
					
						   #Write-Host "Uploding test dependant file " $DependantTestfiles[$i].FullName
						   $wc.UploadFile($UpdateTestURL,$DependantTestfiles[$i].FullName) > $null;		
					   
						}
					}
					#Write-Host "Test files uploaded successfully."
				}
				catch
				{		
				    $statuscode = $_.Exception.Response.StatusCode.value__ ;
					if($statuscode -eq '401')
					{					 
					 Write-Host "Test Result: Unauthorized. Please check API Key and API Secret."; 
					 Write-Host "##vso[task.complete result=Failed;]DONE";		
					 exit 1;					
					}
					else{					 					 
					    Write-Host "Error in updating test. For more details check below error details."; 
					     Write-Host "##vso[task.complete result=Failed;]DONE";	
					     exit 1;
					}					
									
						Write-Host "Error Code:" $_.Exception.Response.StatusCode.value__ 
						Write-Host "Error Details:" $_.ErrorDetails.Message; exit;	
				}
				
				
		}
		catch
		{				
			Write-Host "Error in updating test. Please contact to administrator."				
			Write-Host "Error Code:" $_.Exception.Response.StatusCode.value__ 
			Write-Host "Error Details:" $_.ErrorDetails.Message; 
			Write-Host "##vso[task.complete result=Failed;]DONE";		
            exit 1;
		}
		
		
}

function Is-Numeric ($Value) {
    return $Value -match "^[\d]+$"
}
	
    # Write-Host " inputfile" $inputstartfile
	# Write-Host " apikey" $apikey;
	# Write-Host " apisecret" $apisecret;
	# Write-Host " testurl" $testurl;
	# Write-Host " showtaillog" $showtaillog;
	# Write-Host " createtest" $createtest
	# Write-Host " testname" $testname;
	# Write-Host " inputallfiles" $inputallfiles
	# Write-Host " projectid" $projectid ;
	# Write-Host " totalusers" $totalusers ;

	$StartFileName ="";
	$TestID = 0;
	# $fileExt = [System.IO.Path]::GetExtension($inputfile);
	# $isValidExt =  $fileExt -eq ".jmx"	 
	
try
{ 
	$CreateTestResponse="";
	$BasicAuthKey  =  $apikey  +":" +$apisecret;
	$BasicAuth = [System.Text.Encoding]::UTF8.GetBytes($BasicAuthKey);
	$AuthorizationKey = "Basic "+ [System.Convert]::ToBase64String($BasicAuth) ;
	$hdrs = @{};
	$hdrs.Add("Authorization",$AuthorizationKey);
	 
   
	#$Testid = 0;
	if($createtest -eq "true" )
	{	  
	  $testurl ="";
	  
	  #Write-Host "In create Test"
	  $CreateTestURL = "https://a.blazemeter.com/api/v4/tests";
	 
	 
	 if( $totalusers -eq "" -or  $duration -eq "")
	 {
		$totalusers = 20; $duration =20;
	 }
	 if( $rampup -eq "" )
	 {
		$rampup = 1; 
	 }
	 
	 $fileExt = [System.IO.Path]::GetExtension($inputstartfile);
	 $isValidExt =  $fileExt -eq ".jmx"	
	 $IsValidTotalUsers = Is-Numeric $totalusers ;
	 $IsValidDuration = Is-Numeric $duration ;
	 $IsValidRamup = Is-Numeric $rampup ;
	 
	 
	 if( $fileExt -eq "" )
	  {		
		Write-Host "Please upload start file for test."; 
		Write-Host "##vso[task.complete result=Failed;]DONE";		
        exit 1;
	  }
	  elseif( $IsValidTotalUsers -ne $True )
	  {
	    Write-Host "Invalid total users count."; 
		Write-Host "##vso[task.complete result=Failed;]DONE";		
        exit 1;
	  }
	  elseif( $IsValidDuration  -ne $true )
	  {
	    Write-Host "Invalid total users count." ; 
		Write-Host "##vso[task.complete result=Failed;]DONE";		
        exit 1;
	  }
	  elseif($IsValidRamup -ne $true )
	  {
	    Write-Host "Invalid total users count." ; 
		Write-Host "##vso[task.complete result=Failed;]DONE";		
        exit 1;
	  }
	  else
	  {
			 $duration = $duration - $rampup;
			 $duration =  """$duration m""" -replace (' ')
			 $rampup = """$rampup m"""  -replace (' ')
			
			#Write-Host " rampup" $rampup ;
			#Write-Host " duration" $duration ;
		
		#Write-Host "Found JMX file"
		#Write-Host "Creating new test."
		
		$StartFileName = Split-Path $inputstartfile -leaf
		
	   $json="";
	    if($projectid -ne "" )
		{
		
	   $json = @"
	   {
		"projectId": $projectid ,
		"configuration": {
            "type": "taurus",
			 "filename": "$StartFileName",
			  "scriptType": "jmeter",
            "canControlRampup": false,
            "targetThreads": 275,
            "executionType": "taurusCloud",
            "enableFailureCriteria": true,
            "threads": 275,
            "testMode": "",
            "plugins": {
                "jmeter": {
                    "version": "auto",
                    "consoleArgs": "",
                    "enginesArgs": ""
                },
                "thresholds": {
                     "thresholds": [],
                    "ignoreRampup": false,
                    "slidingWindow": false
                }
            }
        },
		"shouldSendReportEmail": false,
        "overrideExecutions": [
            {
                "concurrency": $totalusers,
                "executor": "",
                "holdFor": $duration  ,
                "locations": {
                    "us-east4-a": $totalusers
                },
                "locationsPercents": {
                    "us-east4-a": 100
                },
                "rampUp": $rampup,
                "steps": 0
            }
        ],
   
		"name": "$testname"
}
"@
		}
		else
		{
			 $json = @"
	   {
	"projectId": null,
	"configuration": {
            "type": "taurus",
			 "filename": "$StartFileName",
			  "scriptType": "jmeter",
            "canControlRampup": false,
            "targetThreads": 275,
            "executionType": "taurusCloud",
            "enableFailureCriteria": true,
            "threads": 275,
            "testMode": "",
            "plugins": {
                "jmeter": {
                    "version": "auto",
                    "consoleArgs": "",
                    "enginesArgs": ""
                },
                "thresholds": {
                     "thresholds": [],
                    "ignoreRampup": false,
                    "slidingWindow": false
                }
            }
        },
     "shouldSendReportEmail": false,
        "overrideExecutions": [
            {
                "concurrency": $totalusers,
                "executor": "",
                "holdFor": $duration ,
                "locations": {
                    "us-east4-a": $totalusers 
                },
                "locationsPercents": {
                    "us-east4-a": 100
                },
                "rampUp": $rampup,
                "steps": 0
            }
        ],
   
		"name": "$testname"
		}
"@
		}
	   #Write-Host "Input to Test " $json
	   try
		{ 
		 
		  $CreateTestResponse = Invoke-RestMethod  $CreateTestURL -Method Post -Body $json -ContentType 'application/json' -Headers $hdrs;	
		  Write-Host "Test:" """$testname""" "created successfully."
		}
		catch
		{
			       $statuscode = $_.Exception.Response.StatusCode.value__ ;
					if($statuscode -eq '401')
					{
					  Write-Host "Test Result: Unauthorized. Please check API Key and API Secret."; 
					  Write-Host "##vso[task.complete result=Failed;]DONE";		
					  exit 1;						 					
					}
					else
					{					    					
					     Write-Host "Unable to start the test. For more details check below error details."; 
					     Write-Host "##vso[task.complete result=Failed;]DONE";		
					     exit 1;
					}
					Write-Host "Error Code:" $_.Exception.Response.StatusCode.value__ 
					Write-Host "Error Details:" $_.ErrorDetails.Message; exit;
		}
	
		if($CreateTestResponse -ne "")
		{   
		    #Write-Host "Test Created."
			$data = $CreateTestResponse | ConvertTo-Json;
			$jsonObj = $data | ConvertFrom-Json;
			$resultObj = $jsonObj.result | ConvertTo-Json;
			$resultObj3 = $resultObj | ConvertFrom-Json;
			$TestID = $resultObj3.id;
			$Isvalidfile = "true"
			if($TestID -gt 0)
			{
			 #Write-Host "Updating the test"
			 #Start-Sleep -Seconds 20; 
			 #Write-Host "Waited for 20 sec"			
			if($inputallfiles -ne "" )
			{ 
			     Write-Host "Updating the test" + $inputallfiles
				 UpdateTest $TestID;
				 $Isvalidfile = CheckIfFileUploadedOrNot $TestID;
				 if($Isvalidfile -eq "true")
				 {
			  
			  StartTest	$TestID "false";
			 }
			  else
			 {
			  Write-Host "Unable to start the test. Check Uploaded file."; 
			  Write-Host "##vso[task.complete result=Failed;]DONE";		
              exit 1;
			 }
			}
			else
			{			 
			  StartTest	$TestID "false";
			}
			
			 
			}
		}
		else
		{
		    Write-Host "Error in starting the test. Please contact to administrator."		; 
			Write-Host "##vso[task.complete result=Failed;]DONE";		
            exit 1;
			
		}
		
	   
	  }
	  	  		
	}
	else
	{
		$uri = $testurl -as [System.URI]
		$checkValidURL = $uri.AbsoluteURI -ne $null -and $uri.Scheme -match '[http|https]';
		
		$GetNumberfromURL  =  $testurl -replace '\D+(\d+)','$1 ';
		$GetIDFromURL = $GetNumberfromURL.split(' ');
		$AccountID = $GetIDFromURL[0];
		$WorkSpaceID = $GetIDFromURL[1];
		$ProjectID = $GetIDFromURL[2];
		$TestID = $GetIDFromURL[3];
		$multitests="false";
		#Write-Host "Found Test URL"
		if($testurl -eq '' )
		{	  		  
		  Write-Host "Enter Test URL."; 
	      Write-Host "##vso[task.complete result=Failed;]DONE";		
	      exit 1;
		}
		# elseif($inputfile -ne "" -and $fileExt -ne "" -and $isValidExt -ne $true )
	    # {		
		# Write-Error "Please upload only JMX file."
	    # }
		elseif($checkValidURL -ne $true )
		{	  
		  Write-Host "Invalid Test URL.";	  
		  Write-Host "##vso[task.complete result=Failed;]DONE";		
	      exit 1;
		}
		elseif(($apikey -eq '$(APIKEY)') -Or ($apikey -eq '$(APISECRET)') )
		{
		  #Write-Host "Please set variable group. Refer this link to create group 'https://docs.microsoft.com/en-us/azure/devops/pipelines/library/variable-groups?view=azure-devops&tabs=yaml'"
		  Write-Host "Please set variable group. Refer this link to create group 'https://docs.microsoft.com/en-us/azure/devops/pipelines/library/variable-groups?view=azure-devops&tabs=yaml'" 
		  Write-Host "##vso[task.complete result=Failed;]DONE";		
	      exit 1;
		}
		elseif($AccountID -eq "" -Or $WorkSpaceID -eq "" -Or $ProjectID -eq "" -Or $TestID -eq "")
		{
		   Write-Host "Unable to start the test. Please check the Test URL.";  
		   Write-Host "##vso[task.complete result=Failed;]DONE";		
	       exit 1;
		}
		else
		{
            if($testurl -like "*multi-tests*" )
			{
		     	$multitests="true"
			}			
			if($Uploadfilechk -eq "true" )
			{ 
				Write-Host "Test Started- Found Valid file" +$Uploadfilechk
				UpdateTest $TestID;
				 $Isvalidfile = CheckIfFileUploadedOrNot $TestID;
				 if($Isvalidfile -eq "true")
				 {
				  Write-Host "Test Started- Found Valid file"
				  StartTest	$TestID $multitests;
				 }
				 else
				 {
				     Write-Host "Unable to start the test."
				 }
			}
			else
			{			 
			  StartTest	$TestID $multitests;
			}
			
			
		}
	
	
	}

}
catch
{
	                 $statuscode = $_.Exception.Response.StatusCode.value__ ;					
					if($statuscode -eq '401')
					{
					  Write-Host "Test Result: Unauthorized. Please check API Key and API Secret."; 
					  Write-Host "##vso[task.complete result=Failed;]DONE";		
					  exit 1;	
					 					
					}
					else
					{					    
					    Write-Host "Unable to start the test. For more details check below error details."
						Write-Host "##vso[task.complete result=Failed;]DONE";		
                        exit 1;
					}
					Write-Host "Error Code:" $_.Exception.Response.StatusCode.value__ 
					Write-Host "Error Details:" $_.ErrorDetails.Message; exit;
}
	
