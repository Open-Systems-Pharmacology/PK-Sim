require 'open-uri'
require 'openssl'
require 'open3'
PKSIM_DB_FILE_NAME = 'PKSimDB.sqlite'
OpenSSL::SSL::VERIFY_PEER = OpenSSL::SSL::VERIFY_NONE

namespace :db do
  desc "Performs a dump of the database"
  task :dump do
    puts "Starting database dump..."
    sh(sqlite3, "#{pksimDb}", ".output #{dump_file}", ".dump", ".exit" )
    puts "Database dump completed and exported under #{dump_file}"
  end
  
  desc "Downloads the develop version of the PKSim Database and performes a diff with the current version of the database"
  task :diff do
    puts "Starting datbase diff..."
    download_pksimdb_file
    stdout, stderr, status = Open3.capture3(sqldiff, "--primarykey", developPKSimDB, pksimDb)
    create_diff_file stdout
    puts "Database diff completed and exported under #{diff_file}"
  end
end

private

def create_diff_file(content)
  File.open(diff_file, 'w') do |file| 
    file.puts content
  end
end

def download_pksimdb_file
  uri = "https://github.com/Open-Systems-Pharmacology/PK-Sim/raw/develop/src/Db/PKSimDB.sqlite"
  puts "Downloading #{PKSIM_DB_FILE_NAME} from #{uri} under #{developPKSimDB}"
  open(developPKSimDB, 'wb') do |fo| 
    fo.print open(uri,:read_timeout => nil).read
  end
end

def pksimDb
  File.join(current_dir, PKSIM_DB_FILE_NAME)
end

def developPKSimDB 
  File.join(diff_dir,'PKSimDB.develop.sqlite')
end

def dump_file
  File.join(dump_dir,'PKSimDB_dump.txt')
end

def diff_file
  File.join(diff_dir,'PKSimDB_diff.txt')
end

def sqldiff
  File.join(diff_dir,'sqldiff.exe')
end

def sqlite3
  File.join(dump_dir,'sqlite3.exe')
end

def dump_dir
  File.join(current_dir,'Dump')
end

def diff_dir
  File.join(current_dir,'Diff')
end

def current_dir
  File.dirname(__FILE__)
end