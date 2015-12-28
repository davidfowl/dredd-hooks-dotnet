Feature: Execution order

  Background:
    Given I have "/Users/vchianese/dev/dredd-hooks-dotnet/bin/output/approot/dredd-hooks-dotnet" command installed
    And I have "/Users/vchianese/dev/dredd-hooks-dotnet/dredd-hooks-test/node_modules/.bin/dredd" command installed
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
      namespace dredd_hooks_dotnet
      {
        public static class Whatever
        {
          public static void Configure(IHooksHandler handler)
          {
            handler.RegisterHandlerFor("/message > GET", EventType.Before, (transaction) => {
            
            });
          }
        }
      }
      """
    Given I set the environment variables to:
      | variable                       | value      |
      | TEST_DREDD_HOOKS_HANDLER_ORDER | true       |

    When I run `/Users/vchianese/dev/dredd-hooks-dotnet/dredd-hooks-test/node_modules/.bin/dredd ./apiary.apib http://localhost:4567 --server "ruby server.rb" --language /Users/vchianese/dev/dredd-hooks-dotnet/bin/output/approot/dredd-hooks-dotnet --hookfiles ./hookfile.cs`
    Then the exit status should be 0
    Then the output should contain:
      """
      0 before all modification
      1 before each modification
      2 before modification
      3 before each validation modification
      4 before validation modification
      5 after modification
      6 after each modification
      7 after all modification
      """
