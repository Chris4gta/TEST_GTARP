node('windows'){
	deleteDir()
	checkout scm
	
	stage('Sonar-Scanner') {			
		if (env.BRANCH_NAME != 'master') {
			if (env.BRANCH_NAME.startsWith('PR-')) {
				bat "SonarQube.Scanner.MSBuild.exe begin /key:terratex:gtmp-rl-rpg /s:${WORKSPACE}/SonarQube.Analysis.xml /version:${BUILD_DISPLAY_NAME} /d:sonar.analysis.mode=preview /d:sonar.github.pullRequest=${CHANGE_ID} /d:sonar.github.oauth=${github_oauth} /d:sonar.github.repository=TerraTex-Community/GTMP-Real--Roleplay-Script"
				bat 'nuget install resources/TerraTex-RL-RPG/packages.config -OutputDirectory resources/packages'
				bat 'msbuild resources/TerraTex-RL-RPG/TerraTex-RL-RPG.csproj'
				bat "SonarQube.Scanner.MSBuild.exe end"			
			} else if (env.BRANCH_NAME == 'develop') {
				withSonarQubeEnv('TerraTex SonarQube') {
					bat "SonarQube.Scanner.MSBuild.exe begin /key:terratex:gtmp-rl-rpg /s:${WORKSPACE}/SonarQube.Analysis.xml /version:${BUILD_DISPLAY_NAME}"
					bat 'nuget install resources/TerraTex-RL-RPG/packages.config -OutputDirectory resources/packages'
					bat 'msbuild resources/TerraTex-RL-RPG/TerraTex-RL-RPG.csproj'
					bat "SonarQube.Scanner.MSBuild.exe end"	
				}					
				
				timeout(time: 1, unit: 'HOURS') {
					def qg = waitForQualityGate()
					if (qg.status != 'OK') {
						error "Pipeline aborted due to quality gate failure: ${qg.status}"
					}
				}
			}
		}
	}
	
	stage('Build') {
		if (env.BRANCH_NAME == 'master') {
			bat 'cd resources/TerraTex-RL-RPG && npm install && npm run-script build'
			bat 'nuget install resources/TerraTex-RL-RPG/packages.config -OutputDirectory resources/packages'
			bat 'msbuild resources/TerraTex-RL-RPG/TerraTex-RL-RPG.csproj'		
		}
		if (env.BRANCH_NAME == 'master' || env.BRANCH_NAME == 'develop') {
			stash includes:'**/*.*', name: 'compiled'
		}
	}
}
node('master') {
	stage('Deploy') {
		if (env.BRANCH_NAME == 'master') {
			unstash 'compiled'		
		
			sh 'ssh root@terratex.eu "rmdir \\"D:/TerraTex/Spiele/GTMP/01_server/live/resources\\" /s /q"'
			sh 'ssh root@terratex.eu "mkdir \\"D:/TerraTex/Spiele/GTMP/01_server/live/resources/TerraTex-RL-RPG\\""'
			sh 'scp -r ./resources/TerraTex-RL-RPG root@terratex.eu:"D:/TerraTex/Spiele/GTMP/01_server/live/resources"'
			
			sh 'ssh root@terratex.eu "xcopy \\"D:/TerraTex/Spiele/GTMP/02_configs/live\\" \\"D:/TerraTex/Spiele/GTMP/01_server/live/resources/TerraTex-RL-RPG/Configs\\" /E /Y /I"'
			sh 'ssh root@terratex.eu "xcopy \\"D:/TerraTex/Spiele/GTMP/03_shared_packages\\" \\"D:/TerraTex/Spiele/GTMP/01_server/live/resources\\" /E /Y /I"'
			
		} else if (env.BRANCH_NAME == 'develop') {		
			unstash 'compiled'
			
			sh 'ssh root@terratex.eu "rmdir \\"D:/TerraTex/Spiele/GTMP/01_server/dev/resources\\" /s /q"'
			sh 'ssh root@terratex.eu "mkdir \\"D:/TerraTex/Spiele/GTMP/01_server/dev/resources/TerraTex-RL-RPG\\""'
			sh 'scp -r ./resources/TerraTex-RL-RPG root@terratex.eu:"D:/TerraTex/Spiele/GTMP/01_server/dev/resources"'
			
			sh 'ssh root@terratex.eu "xcopy \\"D:/TerraTex/Spiele/GTMP/02_configs/dev\\" \\"D:/TerraTex/Spiele/GTMP/01_server/dev/resources/TerraTex-RL-RPG/Configs\\" /E /Y /I"'
			sh 'ssh root@terratex.eu "xcopy \\"D:/TerraTex/Spiele/GTMP/03_shared_packages\\" \\"D:/TerraTex/Spiele/GTMP/01_server/dev/resources\\" /E /Y /I"'
		}
	}

}
