require_relative 'scripts/setup'
require_relative 'scripts/copy-dependencies'
require_relative 'scripts/utils'
require_relative 'scripts/coverage'
require_relative 'src/Db/db'
require 'ostruct'

task :cover do
	filter = []
	filter << "+[PKSim.Core]*"
	filter << "+[PKSim.Assets]*"
	filter << "+[PKSim.Presentation]*"
	filter << "+[PKSim.Infrastructure]*"
  
  #exclude namespaces that are tested from applications
  # filter << "-[OSPSuite.Infrastructure.Serialization]OSPSuite.Infrastructure.Serialization.ORM*"
  # filter << "-[OSPSuite.Presentation]OSPSuite.Presentation.MenuAndBars*"
  # filter << "-[OSPSuite.Presentation]OSPSuite.Presentation.Presenters.ContextMenus*"

  targetProjects = [
	"PKSim.Tests.csproj",
	"PKSim.R.Tests.csproj",
	"PKSim.Matlab.Tests.csproj",
	"PKSim.UI.Tests.csproj",
	];

  Coverage.cover(filter, targetProjects)
end

task :create_setup, [:product_version, :configuration] do |t, args|
	src_dir = src_dir_for(args.configuration)
	relative_src_dir = relative_src_dir_for(args.configuration)

	#Ignore files from automatic harvesting that will be installed specifically
	harvest_ignored_files = [
		'PKSim.exe',
		'PKSimDB.sqlite',
		'PKSimTemplateDBSystem.TemplateDBSystem',
		'templates.json'
	]

	#Files required for setup creation only and that will not be harvested automatically
	setup_files	 = [
		"#{relative_src_dir}/ChartLayouts/**/*.{wxs,xml}",
		"#{relative_src_dir}/TeXTemplates/**/*.*",
		'examples/*.txt',
		'src/PKSim.Assets.Images/Resources/*.ico',
		'Open Systems Pharmacology Suite License.pdf',
		'documentation/*.pdf',
		'dimensions/*.xml',
		'pkparameters/*.xml',
		'setup/setup.wxs',
		'setup/**/*.{msm,rtf,bmp}'
	]


	Rake::Task['setup:create'].execute(OpenStruct.new(
		solution_dir: solution_dir,
		src_dir: src_dir, 
		setup_dir: setup_dir,  
		product_name: product_name, 
		product_version: args.product_version,
		harvest_ignored_files: harvest_ignored_files,		
		suite_name: suite_name,
		setup_files: setup_files,
		manufacturer: manufacturer
		))
end

task :create_portable_setup, [:product_version, :configuration, :package_name] do |t, args|

	src_dir = src_dir_for(args.configuration)
	relative_src_dir = relative_src_dir_for(args.configuration)
	
	# Copy folder structure so that the portable setups works as expected
	FileUtils.mkdir_p setup_temp_dir
	FileUtils.copy_entry File.join(src_dir, 'TeXTemplates'), File.join(setup_temp_dir, 'TeXTemplates')
	FileUtils.copy_entry File.join(src_dir, 'ChartLayouts'), File.join(setup_temp_dir, 'ChartLayouts')


	#Files required for setup creation only and that will not be harvested automatically
	setup_files	 = [
		'Open Systems Pharmacology Suite License.pdf',
		'documentation/*.pdf',
		'dimensions/*.xml',
		'pkparameters/*.xml',
		'setup/**/*.{rtf}'
	]

	setup_folders = [
		'examples/*.md',
		"#{setup_temp_dir}/**/*.*",
	]

	Rake::Task['setup:create_portable'].execute(OpenStruct.new(
		solution_dir: solution_dir,
		src_dir: src_dir_for(args.configuration), 
		setup_dir: setup_dir,  
		product_name: product_name, 
		product_version: args.product_version,
		suite_name: suite_name,
		setup_files: setup_files,
		setup_folders: setup_folders,
		package_name: args.package_name,
		))
end

task :update_go_license, [:file_path, :license] do |t, args|
	Utils.update_go_diagram_license args.file_path, args.license
end	

task :postclean do |t, args| 
	packages_dir =  src_dir_for("Debug")

	all_users_dir = ENV['ALLUSERSPROFILE']
	all_users_application_dir = File.join(all_users_dir, manufacturer, product_name, '12.1')

	copy_dependencies solution_dir,  all_users_application_dir do
		copy_dimensions_xml
		copy_pkparameters_xml
	end

	copy_dependencies solution_dir,  all_users_application_dir do
		copy_file 'src/Db/PKSimDB.sqlite'
		copy_file 'src/Db/TemplateDB/PKSimTemplateDBSystem.templateDBSystem'
	end

	copy_dependencies packages_dir,   File.join(all_users_application_dir, 'ChartLayouts') do
		copy_files 'ChartLayouts', 'xml'
	end

	copy_dependencies packages_dir,   File.join(all_users_application_dir, 'TeXTemplates', 'StandardTemplate') do
		copy_files 'StandardTemplate', '*'
	end
end

task :db_pre_commit do
	Rake::Task['db:dump'].execute();
	Rake::Task['db:diff'].execute();
end

private

def relative_src_dir_for(configuration)
	File.join( 'src', 'PKSim', 'bin', configuration, 'net472')
end

def src_dir_for(configuration)
	File.join(solution_dir, relative_src_dir_for(configuration))
end

def solution_dir
	File.dirname(__FILE__)
end

def	manufacturer
	'Open Systems Pharmacology'
end

def	product_name
	'PK-Sim'
end

def suite_name
	'Open Systems Pharmacology Suite'
end

def setup_dir
	File.join(solution_dir, 'setup')
end

def setup_temp_dir
	File.join(setup_dir, 'temp')
end