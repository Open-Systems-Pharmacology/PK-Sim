require_relative 'scripts/setup'
require_relative 'scripts/copy-dependencies'
require_relative 'scripts/utils'
require_relative 'scripts/coverage'
require_relative 'scripts/smartxls'

task :cover do
	filter = []
	filter << "+[PKSim.Core]*"
	filter << "+[PKSim.Assets]*"
	filter << "+[PKSim.Presentation]*"
	filter << "+[PKSim.Infrastructure]*"

	Coverage.cover(filter, "PKSim.Tests.csproj")
end

task :create_setup, [:product_version, :configuration, :smart_xls_package, :smart_xls_version] do |t, args|
	setup_dir = File.join(solution_dir, 'setup')
	src_dir = File.join(solution_dir, 'src', 'PKSim', 'bin', args.configuration)
	product_version = args.product_version
	suite_name = 'Open Systems Pharmacology Suite'

	SmartXls.update_smart_xls src_dir, args.smart_xls_package, args.smart_xls_version

	#Ignore files from automatic harvesting that will be installed specifically
	harvest_ignored_files = [
		'PKSim.exe' 
	]

	#Files required for setup creation only
	setup_files	 = [
		'packages/**/OSPSuite.Presentation/**/*.{wxs,xml}',
		'packages/**/OSPSuite.TeXReporting/**/*.*',
		'src/Data/Examples/*.*',
		'src/PKSim.Assets/Resources/*.ico',
		'src/Db/PKSimDB.mdb',
		'src/Db/TemplateDB/PKSimTemplateDBSystem.mdb',
		'Open Systems Pharmacology Suite License.pdf',
		'documentation/*.pdf',
		'dimensions/*.xml',
		'pkparameters/*.xml',
		'setup/setup.wxs',
		'setup/**/*.{msm,rtf,bmp}',
		'log4net.config.xml'
	]

	Rake::Task['setup:create'].execute(OpenStruct.new(
		solution_dir: solution_dir,
		src_dir: src_dir, 
		setup_dir: setup_dir,  
		product_name: product_name, 
		product_version: product_version,
		harvest_ignored_files: harvest_ignored_files,		
		suite_name: suite_name,
		setup_files: setup_files,
		manufacturer: manufacturer
		))
end

task :update_go_license, [:file_path, :license] do |t, args|
	Utils.update_go_diagram_license args.file_path, args.license
end	

task :postclean do |t, args| 
	packages_dir =  File.join(solution_dir, 'packages')

	all_users_dir = ENV['ALLUSERSPROFILE']
	all_users_application_dir = File.join(all_users_dir, manufacturer, product_name, '7.1')

	copy_depdencies solution_dir,  all_users_application_dir do
		copy_dimensions_xml
		copy_pkparameters_xml
	end

	copy_depdencies solution_dir,  all_users_application_dir do
		copy_file 'src/Db/PKSimDB.mdb'
		copy_file 'src/Db/TemplateDB/PKSimTemplateDBSystem.mdb'
	end

	copy_depdencies packages_dir,   File.join(all_users_application_dir, 'ChartLayouts') do
		copy_files 'OSPSuite.Presentation', 'xml'
	end

	copy_depdencies packages_dir,   File.join(all_users_application_dir, 'TeXTemplates', 'StandardTemplate') do
		copy_files 'StandardTemplate', '*'
	end
end

private

def solution_dir
	File.dirname(__FILE__)
end

def	manufacturer
	'Open Systems Pharmacology'
end

def	product_name
	'PK-Sim'
end
