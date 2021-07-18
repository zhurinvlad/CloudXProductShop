#!/usr/bin/env groovy

pipeline {
    agent any

    options {
        buildDiscarder(logRotator(numToKeepStr: '10'))
        disableConcurrentBuilds()
        timeout(time: 1, unit: 'HOURS')
        timestamps()
    }

    environment {
        VERSION = '${BUILD_NUMBER}'

        ACCOUNT_ID = '515001955773'
        AWS_ECR_REGION = 'us-east-2'
        AWS_URL = '515001955773.dkr.ecr.us-east-2.amazonaws.com'
        IMAGE_NAME = '515001955773.dkr.ecr.us-east-2.amazonaws.com/cloudx-products'
        TASK_NAME = 'CloudX-container'
        CLUSTER_NAME = 'ECS-CloudXCluster'
        SERVICE_NAME = 'CloudXService'
    }

    stages {
		stage('Clone repository') {
            steps {
                git branch: "main", url: "https://github.com/zhurinvlad/CloudXProductShop.git"
            }
        }
		stage('Tests')
		{
			steps {
                sh 'echo tests'
			}
		}
		stage('Build image') {
			steps {
				sh 'docker build -t ${IMAGE_NAME} -f ./CloudXProductShop/CloudXProductShop/Dockerfile .'
			}
		} 

        stage('Push image') {
			steps {
				script {
					docker.withRegistry('${AWS_URL}', 'ecr:${AWS_ECR_REGION}:jenkins-ecr') {
						docker.image('${IMAGE_NAME}').push("v${env.BUILD_NUMBER}")
						docker.image('${IMAGE_NAME}').push('latest')
					}
				}
			}
		}
		// stage('Run instance')
		// {
		// 	steps {
		// 		//  sh 'sed -e "s;%BUILD_NUMBER%;${BUILD_NUMBER};g" ${TASK_NAME}.json >  ${TASK_NAME}-v${BUILD_NUMBER}.json'
		// 		//  sh 'aws ecs register-task-definition --family {TASK_NAME} --cli-input-json file://${TASK_NAME}-v${BUILD_NUMBER}.json --region ${AWS_ECR_REGION}'
		// 		// sh '''	
		// 		// 	TASK_REVISION=`aws ecs describe-task-definition --task-definition ${TASK_NAME} --region ${AWS_ECR_REGION} | jq .taskDefinition.revision`
		// 		// 	SERVICES=`aws ecs describe-services --services ${SERVICE_NAME} --cluster ${CLUSTER_NAME} --region ${AWS_ECR_REGION} | jq '.services[] | length'`
		// 		// 	if [ $SERVICES == "" ]; then 
		// 		// 		aws ecs create-service --cluster ${CLUSTER_NAME} --region ${AWS_ECR_REGION} --service ${SERVICE_NAME} --task-definition ${TASK_NAME}:${TASK_REVISION} --desired-count 1 
		// 		// 	else 
		// 		// 		aws ecs update-service --cluster ${CLUSTER_NAME} --service ${SERVICE_NAME} --task-definition ${TASK_NAME}:${TASK_REVISION} --desired-count 1 --region ${AWS_ECR_REGION}
		// 		// 	fi
		// 		// '''
		// 	}
		// }
		stage('Clean up')
		{		
			steps {
				sh 'docker image prune -a -f'
				deleteDir()
				script {
					currentBuild.result = 'SUCCESS'
				}
			}
		}
    }

    post {
        failure {
            script {
                currentBuild.result = 'FAILURE'
            }
        }
    }
    
}