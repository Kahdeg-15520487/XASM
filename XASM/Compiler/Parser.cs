using System.Collections.Generic;
using System.Linq;

// Set the name of your grammar here (and at the end of this grammar):


using XASM;

namespace XASM {


namespace Compiler{

public class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _intergerLiteral = 2;
	public const int _floatLiteral = 3;
	public const int _charLiteral = 4;
	public const int _stringLiteral = 5;
	public const int _colon = 6;
	public const int _openBracket = 7;
	public const int _closeBracket = 8;
	public const int _openBrace = 9;
	public const int _closeBrace = 10;
	public const int _leftanglebracket = 11;
	public const int _rightanglebracket = 12;
	public const int maxT = 54;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

	public bool verbose;

ScriptEmitter emitter;// = new ScriptEmit();
	Scope global = new Scope();
	Scope currScope;
	List<HostAPILibrary> hapilibs = new List<HostAPILibrary>();

// If you want your generated compiler case insensitive add the
// keyword IGNORECASE here.


	public Parser(Scanner scanner, ScriptEmitter emitter, Errors errorsStream,bool verbose = false,params HostAPILibrary[] hapilibs) {
		this.scanner = scanner;
		this.emitter = emitter;
		errors = errorsStream;
		this.verbose = verbose;
		foreach (var hapilib in hapilibs)
        {
            this.hapilibs.Add(hapilib);
        }
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void literal(out Value lit) {
		lit = new Value(); string temp; 
		if (la.kind == 2) {
			Get();
			lit = new Value(int.Parse(t.val)); 
		} else if (la.kind == 3) {
			Get();
			lit = new Value(float.Parse(t.val)); 
		} else if (la.kind == 4) {
			Get();
			temp = t.val.Remove(t.val.Length-1,1).Remove(0,1);
			lit = new Value(char.Parse(temp)); 
		} else if (la.kind == 5) {
			Get();
			temp = t.val.Remove(t.val.Length-1,1).Remove(0,1);
			lit = new Value(emitter.AddString(temp),ValType.stringLiteral);
			
		} else if (la.kind == 13 || la.kind == 14) {
			if (la.kind == 13) {
				Get();
			} else {
				Get();
			}
			lit = new Value(bool.Parse(t.val)); System.Console.WriteLine(lit.ToString());
		} else SynErr(55);
	}

	void identifier(out string name) {
		Expect(1);
		name = t.val;
	}

	void operand(out Value operand) {
		operand = new Value(); 
		string name,name2; 
		Value lit,lit2;
		
		if (StartOf(1)) {
			literal(out lit);
			operand.Assign(lit); 
		} else if (la.kind == 1) {
			identifier(out name);
			operand.type = ValType.stackReference;
			if (!currScope.ContainVariable(name) 
			&& !currScope.ContainParameter(name)
			&& (name.CompareTo("retval") != 0))
			SemErr(name + " is not defined");
			operand.s = name;
			
			if (la.kind == 7) {
				Get();
				
				if (StartOf(1)) {
					literal(out lit);
					if (lit.IsInterger())
					    operand.i = lit.i;
					else{
					    SemErr("Array index must be of type intergerLiteral");
					}
				} else if (la.kind == 1) {
					identifier(out name2);
					operand.type = ValType.arrayIndex;
					operand.s +='|' + name2; 
				} else SynErr(56);
				Expect(8);
			}
		} else if (la.kind == 11) {
			Get();
			literal(out lit);
			if (lit.IsInterger()){
			   operand.type = ValType.stackIndex;
			   operand.i = lit.i;
			}
			else{
			      SemErr("Stack direct index must be of type intergerLiteral");
			}
			
			if (la.kind == 15) {
				Get();
				literal(out lit2);
				if (lit2.IsInterger()){
				  operand.i += lit2.i;
				}
				else{
				     SemErr("Stack direct index must be of type intergerLiteral");
				}
				
			}
			Expect(12);
		} else SynErr(57);
	}

	void variableDeclare(out string name) {
		
		Expect(16);
		identifier(out name);
		Value lit;
		currScope.AddVariable(name);
		if (verbose)
		System.Console.WriteLine(name + " " + (currScope.IsGlobalScope ? "global" : "local")); 
		
		if (la.kind == 7) {
			Get();
			literal(out lit);
			Expect(8);
			if (lit.IsInterger())
			{
			   currScope.RemoveVariable(name);
			   currScope.AddArray(name, lit.i);
			}
			else
			{
			   SemErr("Array's capacity must be an intergerLiteral");
			} 
		}
	}

	void parameterDeclare(out string name) {
		Expect(17);
		identifier(out name);
		currScope.AddParameter(name);
		if (verbose)
		System.Console.WriteLine(name + " param"); 
		
	}

	void linelabelDeclare(out string name) {
		identifier(out name);
		Expect(6);
		
	}

	void instruction(out Instruction instr) {
		Value op1,op2,op3; 
		string name;
		instr = new Instruction(OpCode.ret);
		switch (la.kind) {
		case 18: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr = new Instruction(OpCode.mov, op1 ,op2); 
			break;
		}
		case 19: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.add, op1,op2); 
			break;
		}
		case 20: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.sub, op1,op2); 
			break;
		}
		case 21: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.mul, op1,op2); 
			break;
		}
		case 22: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.div, op1,op2); 
			break;
		}
		case 23: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.mod, op1,op2); 
			break;
		}
		case 24: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.exp, op1,op2); 
			break;
		}
		case 25: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.and, op1,op2); 
			break;
		}
		case 26: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.or, op1,op2); 
			break;
		}
		case 27: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.xor, op1,op2); 
			break;
		}
		case 28: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.shl, op1,op2); 
			break;
		}
		case 29: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr  = new Instruction(OpCode.shr, op1,op2); 
			break;
		}
		case 30: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr = new Instruction(OpCode.gettype, op1,op2); 
			break;
		}
		case 31: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			instr = new Instruction(OpCode.concat, op1,op2); 
			break;
		}
		case 32: {
			Get();
			identifier(out name);
			instr  = new Instruction(OpCode.call, new Value(name,ValType.intergerLiteral)); 
			break;
		}
		case 33: {
			string name2;
			Get();
			identifier(out name);
			int tttt = hapilibs.Count(hapilib =>
			{
			return hapilib.ContainsHostAPI(name);
			});
			switch (tttt)
			{
			case 0:
			if (!hapilibs.Any(hapilib => string.Compare(hapilib.HAPILibraryName, name, true) == 0))
			SemErr("There is no HAPI library contains " + name);
			break;
			case 1:
			instr = new Instruction(OpCode.callhost, new Value(emitter.AddHAPI(name)));
			break;
			default:
			SemErr("There is more than 1 HAPI library contains " + name);
			break;
			} 
			if (la.kind == 34) {
				Get();
				identifier(out name2);
				if (hapilibs.FirstOrDefault(hapilib=>
				 {
				     return string.Compare(hapilib.HAPILibraryName, name, true) == 0;
				 }) ==null)
				{
				 SemErr("Missing reference to " + name);
				}
				instr  = new Instruction(OpCode.callhost, new Value(emitter.AddHAPI(name + '.' + name2))); 
			}
			break;
		}
		case 35: {
			Get();
			operand(out op1);
			instr  = new Instruction(OpCode.push, op1); 
			break;
		}
		case 36: {
			Get();
			operand(out op1);
			instr  = new Instruction(OpCode.pop, op1); 
			break;
		}
		case 37: {
			Get();
			identifier(out name);
			instr  = new Instruction(OpCode.jmp,new Value(name)); 
			break;
		}
		case 38: {
			Get();
			operand(out op1);
			instr  = new Instruction(OpCode.neg, op1); 
			break;
		}
		case 39: {
			Get();
			operand(out op1);
			instr  = new Instruction(OpCode.inc, op1); 
			break;
		}
		case 40: {
			Get();
			operand(out op1);
			instr  = new Instruction(OpCode.dec, op1); 
			break;
		}
		case 41: {
			Get();
			operand(out op1);
			instr  = new Instruction(OpCode.not, op1); 
			break;
		}
		case 42: {
			Get();
			operand(out op1);
			instr  = new Instruction(OpCode.pause, op1); 
			break;
		}
		case 43: {
			Get();
			operand(out op1);
			instr  = new Instruction(OpCode.exit, op1); 
			break;
		}
		case 44: {
			Get();
			instr  = new Instruction(OpCode.ret); 
			break;
		}
		case 45: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			Expect(15);
			identifier(out name);
			instr  = new Instruction(OpCode.je,op1,op2,new Value(name)); 
			break;
		}
		case 46: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			Expect(15);
			identifier(out name);
			instr  = new Instruction(OpCode.jne,op1,op2,new Value(name)); 
			break;
		}
		case 47: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			Expect(15);
			identifier(out name);
			instr  = new Instruction(OpCode.jg,op1,op2,new Value(name)); 
			break;
		}
		case 48: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			Expect(15);
			identifier(out name);
			instr  = new Instruction(OpCode.jl,op1,op2,new Value(name)); 
			break;
		}
		case 49: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			Expect(15);
			identifier(out name);
			instr  = new Instruction(OpCode.jge,op1,op2,new Value(name)); 
			break;
		}
		case 50: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			Expect(15);
			identifier(out name);
			instr  = new Instruction(OpCode.jle,op1,op2,new Value(name)); 
			break;
		}
		case 51: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			Expect(15);
			operand(out op3);
			instr  = new Instruction(OpCode.getchar,op1,op2,op3); 
			break;
		}
		case 52: {
			Get();
			operand(out op1);
			Expect(15);
			operand(out op2);
			Expect(15);
			operand(out op3);
			instr  = new Instruction(OpCode.setchar,op1,op2,op3); 
			break;
		}
		default: SynErr(58); break;
		}
		
	}

	void functionDeclare(out Function func) {
		string name; string funcname = string.Empty; Instruction instr;
		List<Instruction> instrs = new List<Instruction>();
		Dictionary<string,int> lineLabels = new Dictionary<string,int>();
		int paramCount = 0,varCount = 0,entry = emitter.CurrentLine;
		Expect(53);
		identifier(out name);
		currScope = global.AddScope(name);		//todo check function duplicate
		funcname = name; 
		Expect(9);
		while (StartOf(2)) {
			if (la.kind == 17) {
				parameterDeclare(out name);
				
			} else if (la.kind == 16) {
				variableDeclare(out name);
				
			} else if (la.kind == 1) {
				linelabelDeclare(out name);
				if (lineLabels.Keys.Contains(name))
				   SemErr("Label already declared");
				else
				   lineLabels.Add(name,entry + instrs.Count); 
				
			} else {
				instruction(out instr);
				instrs.Add(instr); 
			}
		}
		instrs.Add(new Instruction(OpCode.ret)); 
		Expect(10);
		if (verbose){
		      System.Console.WriteLine();
		      System.Console.WriteLine(currScope.ToString());
		}
		
		for (int ic = 0; ic< instrs.Count;ic++){
		   var tempinstr = instrs[ic];
		   switch (tempinstr.opcode)
		   {
		   	case OpCode.jmp:
		   		instrs[ic].operands[0] = new Value(lineLabels[tempinstr.operands[0].s]);
		   		break;
		
		   	case OpCode.je:
		   	case OpCode.jne:
		   	case OpCode.jl:
		   	case OpCode.jg:
		   	case OpCode.jle:
		   	case OpCode.jge:
		   		instrs[ic].operands[2] = new Value(lineLabels[tempinstr.operands[2].s]);
		   		break;
		
		   	default:
		   		break;
		   }
		   for (int oc = 0; oc<instrs[ic].operands.GetLength(0);oc++){
		       var tempop = instrs[ic].operands[oc];
		          switch (tempop.type){
		              case ValType.stackReference:
		                  if (currScope.ContainVariable(tempop.s))
		                      instrs[ic].operands[oc].i = currScope.GetStackIndexOfVariable(tempop.s) + tempop.i;
		                  else instrs[ic].operands[oc].i = currScope.GetStackIndexOfParameter(tempop.s);
		                  break;
		              case ValType.arrayIndex:
		                  var arrayindex = tempop.s.Split('|');
		                  if (currScope.ContainVariable(arrayindex[0])){
		                      instrs[ic].operands[oc].i = currScope.GetStackIndexOfVariable(arrayindex[0]);
		                      instrs[ic].operands[oc].arrid = currScope.GetStackIndexOfVariable(arrayindex[1]);
		                  }
		                  break;
		          }
		      }
		      emitter.AddInstruction(instrs[ic]);
		      if (verbose)
		          System.Console.WriteLine(emitter.CurrentLine-1 + " : " + instrs[ic]);
		  }
		  int stackeval = 0;
		  int pushcount = 0;
		  int popcount = 0;
		  int callhost_count = 0;
		  for (int ic = 0; ic< instrs.Count;ic++){
		      switch (instrs[ic].opcode){
		          case OpCode.push:
		              stackeval++;
		              pushcount++;
		              break;
		          case OpCode.pop:
		              stackeval--;
		              popcount++;
		              break;
		          case OpCode.callhost:
		              HostAPI hapi;
		              string hapiname = emitter.hapitable[instrs[ic].operands[0].i];
		              if (hapiname.Contains('.'))
		              {
		                  var tttt = hapiname.Split('.');
		                  hapi = hapilibs.First(lib =>
		                  {
		                      return string.Compare(lib.HAPILibraryName, tttt[0], true) == 0;
		                  }).GetAllHostAPI().First(h =>
		                  {
		                      return string.Compare(h.HAPIname, tttt[1], true) == 0;
		                  });
		              }
		              else
		              {
		                  hapi = hapilibs.First(lib =>
		                  {
		                      return lib.ContainsHostAPI(hapiname);
		                  }).GetAllHostAPI().First(h =>
		                  {
		                      return string.Compare(h.HAPIname, hapiname, true) == 0;
		                  });
		              }
		              stackeval -= hapi.paramCount;
		              callhost_count+=hapi.paramCount;
		              break;
		      }
		  }
		
		  if (stackeval < 0)
		  {
		      SemErr(string.Format("Stack corruption: pop > push{0} push count: {1}{0} pop count: {2}{0} callhost count: {3}{0}",System.Environment.NewLine,pushcount,popcount,callhost_count));
		  }
		  else
		  {
		      if (stackeval > 0)
		      {
		          SemErr(string.Format("Stack corruption: pop < push{0} push count: {1}{0} pop count: {2}{0} callhost count: {3}{0}",System.Environment.NewLine,pushcount,popcount,callhost_count));
		      }
		  }
		  varCount = currScope.variables.Count;
		  paramCount = currScope.parameters.Count;
		  func = new Function(entry,paramCount,varCount,funcname); 
	}

	void XASM() {
		string name; Function func; currScope = global; 
		currScope.AddVariable("retval"); 
		while (la.kind == 16 || la.kind == 53) {
			if (la.kind == 16) {
				variableDeclare(out name);
				
			} else {
				functionDeclare(out func);
				int funcindex = emitter.AddFunction(func);
				if (verbose){
				System.Console.WriteLine("entry : " + func.entryPoint);
				System.Console.WriteLine(func.paramCount + " parameter");
				System.Console.WriteLine(func.varCount + " local variable");
				}
				  if (string.Compare(func.funcName,"main")==0) {
				    emitter.mainFuncIndex = funcindex;
				 }
				
			}
		}
		emitter.AddInstruction(new Instruction(OpCode.exit,new Value(0)));
		emitter.globalDataSize = global.variables.Count; 
		if (verbose){
		System.Console.WriteLine("globalDataSize =" + emitter.globalDataSize);
		System.Console.WriteLine("mainFuncIndex =" + emitter.mainFuncIndex);
		}
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		XASM();
		Expect(0);

    Expect(0);
	}
	
	bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream;				// error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text
	
	public Errors(){errorStream = System.Console.Out;}
	public Errors(System.IO.TextWriter tw){errorStream = tw;}

	public void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "intergerLiteral expected"; break;
			case 3: s = "floatLiteral expected"; break;
			case 4: s = "charLiteral expected"; break;
			case 5: s = "stringLiteral expected"; break;
			case 6: s = "colon expected"; break;
			case 7: s = "openBracket expected"; break;
			case 8: s = "closeBracket expected"; break;
			case 9: s = "openBrace expected"; break;
			case 10: s = "closeBrace expected"; break;
			case 11: s = "leftanglebracket expected"; break;
			case 12: s = "rightanglebracket expected"; break;
			case 13: s = "\"true\" expected"; break;
			case 14: s = "\"false\" expected"; break;
			case 15: s = "\",\" expected"; break;
			case 16: s = "\"var\" expected"; break;
			case 17: s = "\"param\" expected"; break;
			case 18: s = "\"mov\" expected"; break;
			case 19: s = "\"add\" expected"; break;
			case 20: s = "\"sub\" expected"; break;
			case 21: s = "\"mul\" expected"; break;
			case 22: s = "\"div\" expected"; break;
			case 23: s = "\"mod\" expected"; break;
			case 24: s = "\"exp\" expected"; break;
			case 25: s = "\"and\" expected"; break;
			case 26: s = "\"or\" expected"; break;
			case 27: s = "\"xor\" expected"; break;
			case 28: s = "\"shl\" expected"; break;
			case 29: s = "\"shr\" expected"; break;
			case 30: s = "\"gettype\" expected"; break;
			case 31: s = "\"concat\" expected"; break;
			case 32: s = "\"call\" expected"; break;
			case 33: s = "\"callhost\" expected"; break;
			case 34: s = "\".\" expected"; break;
			case 35: s = "\"push\" expected"; break;
			case 36: s = "\"pop\" expected"; break;
			case 37: s = "\"jmp\" expected"; break;
			case 38: s = "\"neg\" expected"; break;
			case 39: s = "\"inc\" expected"; break;
			case 40: s = "\"dec\" expected"; break;
			case 41: s = "\"not\" expected"; break;
			case 42: s = "\"pause\" expected"; break;
			case 43: s = "\"exit\" expected"; break;
			case 44: s = "\"ret\" expected"; break;
			case 45: s = "\"je\" expected"; break;
			case 46: s = "\"jne\" expected"; break;
			case 47: s = "\"jg\" expected"; break;
			case 48: s = "\"jl\" expected"; break;
			case 49: s = "\"jge\" expected"; break;
			case 50: s = "\"jle\" expected"; break;
			case 51: s = "\"getchar\" expected"; break;
			case 52: s = "\"setchar\" expected"; break;
			case 53: s = "\"func\" expected"; break;
			case 54: s = "??? expected"; break;
			case 55: s = "invalid literal"; break;
			case 56: s = "invalid operand"; break;
			case 57: s = "invalid operand"; break;
			case 58: s = "invalid instruction"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: System.Exception {
	public FatalError(string m): base(m) {}
}
}}