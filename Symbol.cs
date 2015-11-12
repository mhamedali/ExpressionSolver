﻿using System.Collections;

namespace AK
{
	
	public enum SymbolType
	{
		Empty,
		Value,
		OperatorAdd,
		OperatorMultiply,
		OperatorDivide,
		SubExpression,
		FuncPow,
		FuncSin,
		FuncCos,
		FuncAbs,
		FuncTan,
		FuncCustom,
	};
	
	public class Symbol
	{
		public SymbolType type;
		public double _value;
		public Variable variable;
		public SymbolList subExpression;
		public CustomFunction customFunc;

		// Test if value of the symbol is independent of variables.
		public bool IsImmutableConstant()
		{
			if (type == SymbolType.Value)
			{
				if (variable != null)
					return false;
				return true;
			}
			else if (type == SymbolType.SubExpression)
			{
				return IsSymbolListImmutableConstant(subExpression);
			}
			else
			{
				return false;
			}
		}

		public bool IsMonome() {
			var s = this;
			if (s.type==SymbolType.Value)
			{
				return true;
			}
			else if (s.type==SymbolType.SubExpression) 
			{
				var syms = s.subExpression;
				for (int i=0;i<syms.Length;i++)
				{
					var r = syms.getSymbol(i);
					if (r.type!=SymbolType.Value && r.type!=SymbolType.SubExpression)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public void Simplify() {
			// ((x)) ==> (x)
			if (type == SymbolType.SubExpression) {
				if (subExpression.Length == 1 && subExpression.first.type == SymbolType.SubExpression) {
					// Get pointer to sub-subexpression
					SymbolList subSubExpression = subExpression.symbols[0].subExpression;
					subExpression = subSubExpression;
				}
				else if (subExpression.Length == 1 && subExpression.first.type == SymbolType.Value) {
					// We have single real number surrounded by parenthesis, it can become a real number
					CopyValuesFrom(subExpression.first);
				}
			}
		}
		
		private bool IsSymbolListImmutableConstant(SymbolList l)
		{
			for (int k = 0; k < l.Length; k++) {
				var s = l.getSymbol(k);
				if (s.type == SymbolType.Value) {
					if (s.variable != null) {
						return false;
					}
				}
				else if (l.getSymbol(k).type == SymbolType.SubExpression) {
					if (!IsSymbolListImmutableConstant(s.subExpression)) {
						return false;
					}
				}
			}
			return true;
		}
		
		public void CopyValuesFrom(Symbol o)
		{
			type = o.type;
			_value = o._value;
			variable = o.variable;
			subExpression = o.subExpression;
			customFunc = o.customFunc;
		}
		
		public double value 
		{
			get
			{
				return variable == null ? _value : variable.value;
			}
		}
		
		public bool IsValueType() {
			return type == SymbolType.Value || type == SymbolType.SubExpression;
		}
		
		public Symbol(SymbolType type, double va)
		{
			this.type = type;
			_value = va;
			variable = null;
		}
		
		public Symbol(SymbolType type) {
			this.type = type;
		}
		
		public Symbol(double value) {
			type = SymbolType.Value;
			_value = value;
			variable = null;
		}
		
		public Symbol(Variable ptrToConstValue) {
			type = SymbolType.Value;
			variable = ptrToConstValue;
		}
		
		public Symbol(CustomFunction func)
		{
			type = SymbolType.FuncCustom;
			customFunc = func;
		}
		
		public override string ToString()
		{
			switch (type) {
				case SymbolType.Value:
					if (variable != null)
					{
						return variable.name;
					}
					return _value.ToString();
				case SymbolType.OperatorAdd:
					return "+";
				case SymbolType.OperatorMultiply:
					return "*";
				case SymbolType.OperatorDivide:
					return "/";
				case SymbolType.FuncSin:
					return "sin";
				case SymbolType.FuncCos:
					return "cos";
				case SymbolType.FuncPow:
					return "pow";
				case SymbolType.FuncTan:
					return "builtintan";
				case SymbolType.FuncAbs:
					return "abs";
				case SymbolType.SubExpression:
					return "("+subExpression.ToString()+")";
				case SymbolType.Empty:
					return "(null)";
				case SymbolType.FuncCustom:
					return customFunc.name;
			}
			return "";
		}
	}

}
