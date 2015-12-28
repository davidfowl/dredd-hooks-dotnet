Feature: Failing a transaction

  Background:
    Given I have "dredd-hooks-dotnet" command installed
    And I have "dredd" command installed
    And a file named "server.rb" with:
      """
      require 'sinatra'
      get '/message' do
        "Hello World!\n\n"
      end
      """

    And a file named "apiary.apib" with:
      """
      # My Api
      ## GET /message
      + Response 200 (text/html;charset=utf-8)
          Hello World!
      """

  @debug
  Scenario:
    Given a file named "hookfile.cs" with:
      """
      ## Implement before hook failing the transaction by setting string 'Yay! Failed!' as value of key 'fail'
      ## in the transaction object
      ##
      ## So, replace following pseudo code with yours:
      #
      #require 'mylanguagehooks'
      #
      #before("/message > GET") { |transaction|
      #  transaction['fail'] == 'Yay! Failed!'
      #}
      #
      """
    When I run `dredd ./apiary.apib http://localhost:4567 --server "ruby server.rb" --language "dredd-hooks-dotnet" --hookfiles ./hookfile.cs`
    Then the exit status should be 1
    And the output should contain:
      """
      Yay! Failed!
      """
