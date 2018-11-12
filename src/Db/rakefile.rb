namespace :db do
	desc "Performs a dump of the database"
	task :dump do
		sh(sqlite3, "#{pksimDb}", ".output #{dump_file}", ".dump", ".exit" )
	end
	
	desc "Performs a diff of the database"
	task :diff do
		puts "DIFF"
	end
end

def sqlite3
  File.join(dump_dir,'sqlite3.exe')
end

def dump_commands
	File.join(dump_dir,'commands_dumpdb.txt')
end

def pksimDb
	File.join(current_dir,'PKSimDB.sqlite')
end

def dump_dir
	File.join(current_dir,'Dump')
end

def diff_dir
	File.join(current_dir,'Diff')
end

def dump_file
	File.join(dump_dir,'PKSimDB_dump.txt')
end

def current_dir
	File.dirname(__FILE__)
end